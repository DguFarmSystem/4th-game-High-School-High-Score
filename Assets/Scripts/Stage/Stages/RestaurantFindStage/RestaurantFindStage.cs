using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;
using UnityEngine.UI;

public class RestaurantFindStage : StageNormal
{
    [Header("접시 프리팹")]
    [SerializeField] private GameObject eggplatePrefab;
    [SerializeField] private GameObject omuricePrefab;
    [SerializeField] private GameObject parfaitPrefab;
    [SerializeField] private GameObject sushiPrefab;
    
    [Header("UI References")]
    [SerializeField] private Transform platesContainer; // Canvas 안의 접시 컨테이너
    [SerializeField] private Vector3[] platePositions = new Vector3[3]; // 3개 접시 위치 (RectTransform)
    [SerializeField] private Image wantedPosterFood; // Wanted 포스터 Image
    
    [Header("포스터")]
    [SerializeField] private Sprite eggPlateWantedSprite; // eggplate 포스터
    [SerializeField] private Sprite omuriceWantedSprite; // omurice 포스터
    [SerializeField] private Sprite parfaitWantedSprite; // parfait 포스터
    [SerializeField] private Sprite sushiWantedSprite; // sushi 포스터
    
    [Header("애니메이션 속도")]
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
    
    private void SpawnPlatesWithTarget()
    {
        GameObject[] allPrefabs = { eggplatePrefab, omuricePrefab, parfaitPrefab, sushiPrefab };
        Sprite[] allWantedSprites = { eggPlateWantedSprite, omuriceWantedSprite, parfaitWantedSprite, sushiWantedSprite };
        
        // 포스터 랜덤 선택
        targetFoodIndex = Random.Range(0, 4);
        
        // Wanted 포스터 표시
        if (wantedPosterFood != null)
        {
            wantedPosterFood.sprite = allWantedSprites[targetFoodIndex];
        }
        
        // 접시 3개 선택
        List<int> selectedIndices = SelectThreePlatesWithTarget(targetFoodIndex);
        
        // 정답 접시 위치 결정
        correctPlateIndex = selectedIndices.IndexOf(targetFoodIndex);
        
        // 접시 3개 소환
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
    
    // 접시 닫기
    private IEnumerator CloseLids()
    {
        foreach (var plate in spawnedPlates)
        {
            plate.CloseLid();
        }

        yield return new WaitForSeconds(lidCloseDuration);
    }
    
    // 섞기
    private IEnumerator ShufflePlates()
    {
        if (shuffleAnimator != null)
        {
            shuffleAnimator.SetTrigger("Shuffle");
        }

        yield return new WaitForSeconds(shuffleDuration);
    }
    
    // 타이머 시작
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
    
    /// 플레이어가 접시 선택
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
    
    /// 선택 처리
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
}
