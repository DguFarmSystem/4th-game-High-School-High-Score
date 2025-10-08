using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;
using UnityEngine.UI;

public class RestaurantFindStage : StageNormal
{
    [Header("Plate Prefabs - 4개 프리팹")]
    [SerializeField] private GameObject eggplatePrefab; // 에그플레이트 접시 프리팹
    [SerializeField] private GameObject omuricePrefab; // 오므라이스 접시 프리팹
    [SerializeField] private GameObject parfaitPrefab; // 파르페 접시 프리팹
    [SerializeField] private GameObject sushiPrefab; // 스시 접시 프리팹
    
    [Header("UI References")]
    [SerializeField] private Transform platesContainer; // Canvas 안의 접시 컨테이너
    [SerializeField] private Vector3[] platePositions = new Vector3[3]; // 3개 접시 위치 (RectTransform)
    [SerializeField] private Image wantedPosterFood; // Wanted 포스터 Image
    
    [Header("Wanted Poster Sprites - 4개")]
    [SerializeField] private Sprite eggPlateWantedSprite; // eggplate 포스터
    [SerializeField] private Sprite omuriceWantedSprite; // omurice 포스터
    [SerializeField] private Sprite parfaitWantedSprite; // parfait 포스터
    [SerializeField] private Sprite sushiWantedSprite; // sushi 포스터
    
    [Header("Animation Settings")]
    [SerializeField] private float showFoodDuration = 2f;
    [SerializeField] private float lidCloseDuration = 1f;
    [SerializeField] private Animator shuffleAnimator;
    [SerializeField] private float shuffleDuration = 3f;
    
    private List<PlateController> spawnedPlates = new List<PlateController>();
    private int targetFoodIndex; // 0:egg, 1:omurice, 2:parfait, 3:sushi
    private int correctPlateIndex; // 정답 접시 인덱스
    
    private bool canSelectPlate = false;
    private bool hasSelected = false;
    
    private enum GameState
    {
        ShowingFood,
        ClosingLid,
        Shuffling,
        WaitingSelection,
        Revealing,
        Ended
    }
    private GameState currentGameState = GameState.ShowingFood;
    
    // TEST CODE
    [SerializeField] private GameObject _greenSphere;
    [SerializeField] private GameObject _redSphere;
    
    public override void OnStageStart()
    {
        CurrentStageState = StageState.Playing;
    }
    
    protected override void OnStageEnd()
    {
        base.OnStageEnd();
    }
    
    protected override void OnStageClear()
    {
        base.OnStageClear();
        Debug.Log("Restaurant Find Stage Clear!");
    }
    
    private void OnStageEndedGimmik(bool isStageCleared)
    {
        if (isStageCleared)
        {
            Debug.Log("Stage cleared!");
            if (_greenSphere != null) _greenSphere.SetActive(true);
        }
        else
        {
            Debug.Log("Stage failed!");
            if (_redSphere != null) _redSphere.SetActive(true);
        }
    }
    
    /// <summary>
    /// 게임 시퀀스
    /// </summary>
    private IEnumerator GameSequence()
    {
        // 1. Wanted 포스터 랜덤 선택 & 접시 소환
        SpawnPlatesWithTarget();
        
        // 2. 음식 보여주기
        currentGameState = GameState.ShowingFood;
        yield return new WaitForSeconds(showFoodDuration);
        
        // 3. 뚜껑 닫기
        currentGameState = GameState.ClosingLid;
        yield return StartCoroutine(CloseLids());
        
        // 4. 섞기
        currentGameState = GameState.Shuffling;
        yield return StartCoroutine(ShufflePlates());
        
        // 5. 선택 대기 & 타이머 시작
        currentGameState = GameState.WaitingSelection;
        canSelectPlate = true;
        StartCoroutine(StartGameTimer());
    }
    
    /// <summary>
    /// Wanted 포스터 선택 & 접시 3개 소환
    /// 중요: 3개 중 최소 1개는 Wanted와 같은 음식!
    /// </summary>
    private void SpawnPlatesWithTarget()
    {
        GameObject[] allPrefabs = { eggplatePrefab, omuricePrefab, parfaitPrefab, sushiPrefab };
        Sprite[] allWantedSprites = { eggPlateWantedSprite, omuriceWantedSprite, parfaitWantedSprite, sushiWantedSprite };
        
        // 1. Wanted 포스터 랜덤 선택 (4개 중 1개)
        targetFoodIndex = Random.Range(0, 4);
        
        // Wanted 포스터 표시
        if (wantedPosterFood != null)
        {
            wantedPosterFood.sprite = allWantedSprites[targetFoodIndex];
        }
        
        // 2. 접시 3개 선택 (4개 중 3개, 단 targetFood는 무조건 포함!)
        List<int> selectedIndices = SelectThreePlatesWithTarget(targetFoodIndex);
        
        // 3. 정답 접시 위치 결정 (선택된 3개 중 targetFood가 있는 위치)
        correctPlateIndex = selectedIndices.IndexOf(targetFoodIndex);
        
        // 4. 접시 3개 소환
        for (int i = 0; i < 3; i++)
        {
            int foodIndex = selectedIndices[i];
            GameObject prefab = allPrefabs[foodIndex];
            
            // 프리팹 인스턴스화
            GameObject plateObj = Instantiate(prefab, platesContainer);
            
            // 위치 설정 (RectTransform)
            RectTransform rectTransform = plateObj.GetComponent<RectTransform>();
            if (rectTransform != null && i < platePositions.Length)
            {
                rectTransform.anchoredPosition = platePositions[i];
            }
            
            // PlateController 설정
            PlateController plateController = plateObj.GetComponent<PlateController>();
            if (plateController != null)
            {
                plateController.SetCorrectPlate(i == correctPlateIndex);
                spawnedPlates.Add(plateController);
            }
        }
    }
    
    /// <summary>
    /// 4개 중 3개 선택, 단 targetFoodIndex는 무조건 포함!
    /// </summary>
    private List<int> SelectThreePlatesWithTarget(int targetIndex)
    {
        List<int> result = new List<int>();
        List<int> others = new List<int>();
        
        // targetIndex는 무조건 포함
        result.Add(targetIndex);
        
        // 나머지 3개 중 2개 선택
        for (int i = 0; i < 4; i++)
        {
            if (i != targetIndex)
            {
                others.Add(i);
            }
        }
        
        // 랜덤으로 2개 선택
        for (int i = 0; i < 2; i++)
        {
            int randomIndex = Random.Range(0, others.Count);
            result.Add(others[randomIndex]);
            others.RemoveAt(randomIndex);
        }
        
        // 섞기 (순서 랜덤화)
        for (int i = 0; i < result.Count; i++)
        {
            int temp = result[i];
            int randomIndex = Random.Range(0, result.Count);
            result[i] = result[randomIndex];
            result[randomIndex] = temp;
        }
        
        return result;
    }
    
    /// <summary>
    /// 뚜껑 닫기
    /// </summary>
    private IEnumerator CloseLids()
    {
        foreach (var plate in spawnedPlates)
        {
            plate.CloseLid();
        }
        
        yield return new WaitForSeconds(lidCloseDuration);
    }
    
    /// <summary>
    /// 접시 섞기
    /// </summary>
    private IEnumerator ShufflePlates()
    {
        if (shuffleAnimator != null)
        {
            shuffleAnimator.SetTrigger("Shuffle");
        }
        
        yield return new WaitForSeconds(shuffleDuration);
    }
    
    /// <summary>
    /// 타이머 시작
    /// </summary>
    private IEnumerator StartGameTimer()
    {
        yield return new WaitForSeconds(timerTime);
        
        if (CurrentStageState == StageState.Playing && !hasSelected)
        {
            canSelectPlate = false;
            currentGameState = GameState.Ended;
            CurrentStageState = StageState.Over;
            OnStageEnd();
        }
    }
    
    /// <summary>
    /// 플레이어가 접시 선택
    /// </summary>
    public void OnPlateSelected(PlateController selectedPlate)
    {
        if (!canSelectPlate || hasSelected || currentGameState != GameState.WaitingSelection)
        {
            return;
        }
        
        hasSelected = true;
        canSelectPlate = false;
        
        StartCoroutine(HandlePlateSelection(selectedPlate));
    }
    
    /// <summary>
    /// 선택 처리
    /// </summary>
    private IEnumerator HandlePlateSelection(PlateController selectedPlate)
    {
        currentGameState = GameState.Revealing;
        
        // 선택 표시
        selectedPlate.ShowSelected();
        yield return new WaitForSeconds(0.7f);
        
        // 뚜껑 열기
        selectedPlate.OpenLid();
        yield return new WaitForSeconds(lidCloseDuration);
        
        // 정답 확인
        if (selectedPlate.IsCorrectPlate)
        {
            OnStageClear();
        }
        else
        {
            CurrentStageState = StageState.Over;
            OnStageEnd();
        }
        
        currentGameState = GameState.Ended;
    }
    
    // ============ Lifecycle methods ============ //
    void Awake()
    {
        if (eggplatePrefab == null || omuricePrefab == null || 
            parfaitPrefab == null || sushiPrefab == null)
        {
            Debug.LogError("Plate prefabs are not properly set!");
        }
        
        if (eggPlateWantedSprite == null || omuriceWantedSprite == null || 
            parfaitWantedSprite == null || sushiWantedSprite == null)
        {
            Debug.LogError("Wanted sprites are not properly set!");
        }
    }
    
    void OnEnable()
    {
        OnStageEnded += OnStageEndedGimmik;
    }
    
    void OnDisable()
    {
        OnStageEnded -= OnStageEndedGimmik;
    }
    
    void Start()
    {
        OnStageStart();
        StartCoroutine(GameSequence());
    }
    
    void Update()
    {
        // 필요한 업데이트 로직
    }
}
