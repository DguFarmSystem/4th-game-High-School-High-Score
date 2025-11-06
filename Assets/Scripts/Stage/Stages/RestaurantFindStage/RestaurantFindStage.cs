using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;

public class RestaurantFindStage : StageNormal
{
    private int n; // 접시 개수 (레벨에 따라 자동 설정)
    private int shuffleCount; // 섞기 횟수 (레벨에 따라 자동 설정)
    private float totalShuffleDuration = 2f; // 전체 섞기 시간 (모든 레벨 공통 2초)
    
    [Header("접시 프리팹")]
    [SerializeField] private GameObject eggplatePrefab;
    [SerializeField] private GameObject omuricePrefab;
    [SerializeField] private GameObject parfaitPrefab;
    [SerializeField] private GameObject sushiPrefab;
    
    [Header("접시, 포스터 위치")]
    [SerializeField] private Transform platesContainer; // 접시 컨테이너
    [SerializeField] private SpriteRenderer wantedPosterFood; // Wanted 포스터
    
    [Header("접시 위치 설정")]
    [SerializeField] private Vector3[] platePositions_2; // 레벨2 위치
    [SerializeField] private Vector3[] platePositions_3; // 레벨3 위치
    [SerializeField] private Vector3[] platePositions_4; // 레벨4 위치
    
    [Header("포스터")]
    [SerializeField] private Sprite eggPlateWantedSprite; // eggplate 포스터
    [SerializeField] private Sprite omuriceWantedSprite; // omurice 포스터
    [SerializeField] private Sprite parfaitWantedSprite; // parfait 포스터
    [SerializeField] private Sprite sushiWantedSprite; // sushi 포스터
    
    [Header("애니메이션 속도")]
    [SerializeField] private float showFoodDuration = 2f; // 처음 음식 공개 시간
    [SerializeField] private float lidCloseDuration = 0.5f; // 뚜껑 닫기 시간
    private float swapDuration; // 각 섞기당 걸리는 시간 (레벨에 따라 자동 계산)
    
    [Header("화면 비율 대응")]
    [SerializeField] private float plateScaleMultiplier = 3f; // 접시 크기 조정 배율 (레벨 1-3)
    [SerializeField] private float plateScaleMultiplier_Level4 = 1f; // 접시 크기 조정 배율 (레벨 4)
    [SerializeField] private float posterScaleMultiplier = 0.5f; // 포스터 크기 조정 배율
    
    public float LidCloseDuration => lidCloseDuration; // PlateController에서 참조 가능하도록
    
    private List<PlateController> spawnedPlates = new List<PlateController>(); // 화면에 생성된 접시 리스트
    private int targetFoodIndex; // 0:egg, 1:omurice, 2:parfait, 3:sushi
    
    private bool canSelectPlate = false; // 현재 선택 가능한지
    private bool hasSelected = false; // 중복 선택 금지 (따닥)
    
    private enum GameState
    {
        ShowingFood, // 음식이 보이는 첫 단계
        ClosingLid, // 뚜껑 닫는 과정
        Shuffling, // 섞습니다
        WaitingSelection, // 선택 대기
        Revealing, // 선택 접시 오픈
        Ended // 끝
    }
    private GameState currentGameState = GameState.ShowingFood;
    
    public override void OnStageStart()
    {
        base.OnStageStart(); // StageNormal의 타이머 시작
        CurrentStageState = StageState.Playing;
        
        // 레벨에 따라 설정 초기화
        InitializeLevelSettings();
    }
    
    // 레벨에 따라 접시 개수, 섞기 횟수, 섞기 속도 설정 (1~4)
    private void InitializeLevelSettings()
    {
        int stageLevel = StageManager.Instance.GetDifficulty();
        
        switch (stageLevel)
        {
            case 1:
                n = 2;
                shuffleCount = 1;
                break;
            case 2:
                n = 3;
                shuffleCount = 2;
                break;
            case 3:
                n = 3;
                shuffleCount = 3;
                break;
            case 4:
                n = 4;
                shuffleCount = 4;
                break;
        }
        
        // 각 섞기당 걸리는 시간 = 전체 시간 / 섞기 횟수
        swapDuration = totalShuffleDuration / shuffleCount;
    }
    
    protected override void OnStageEnd() // 실패시
    {
        // 타임 오버 시 정답 접시 보여주기
        if (currentGameState == GameState.WaitingSelection && !hasSelected)
        {
            StartCoroutine(ShowCorrectPlateOnTimeout());
        }
        else
        {
            base.OnStageEnd();
        }
    }
    
    // 타임 오버 시 정답 접시 공개
    private IEnumerator ShowCorrectPlateOnTimeout()
    {
        canSelectPlate = false;
        currentGameState = GameState.Revealing;
        
        // 정답 접시 찾기
        PlateController correctPlate = spawnedPlates.Find(plate => plate.IsCorrectPlate);
        
        if (correctPlate != null)
        {
            // 뚜껑 올리기 (이미지 교체 포함)
            correctPlate.RaiseLid();
            yield return new WaitForSeconds(lidCloseDuration);
        }
        
        // 실패 처리
        base.OnStageEnd();
        currentGameState = GameState.Ended;
    }

    protected override void OnStageClear() //성공시
    {
        base.OnStageClear();
    }
    
    // 일단은 디버그 로그에만 출력
    private void OnStageEndedGimmik(bool isStageCleared)
    {
        if (isStageCleared) // 성공 기믹 (아직까진 로그에)
        {
            Debug.Log("성공!");
            StageManager.Instance.StageClear(true);
        }
        else // 실패 기믹
        {
            Debug.Log("실패!");
            StageManager.Instance.StageClear(false);
        }
    }
    
    private IEnumerator GameSequence(int n)
    {
        // 포스터 랜덤 선택 & 접시 소환
        SpawnPlatesWithTarget(n);
        
        // 음식 보여주기
        currentGameState = GameState.ShowingFood;
        yield return new WaitForSeconds(showFoodDuration);
        
        // 뚜껑 닫기
        currentGameState = GameState.ClosingLid;
        yield return StartCoroutine(CloseLids());
        
        // 섞기
        currentGameState = GameState.Shuffling;
        yield return StartCoroutine(ShufflePlates());
        
        // 선택 대기 (StageNormal의 타이머가 이미 작동 중)
        currentGameState = GameState.WaitingSelection;
        canSelectPlate = true;
    }
    
    private void SpawnPlatesWithTarget(int n) // 접시 개수
    {
        GameObject[] allPrefabs;
        allPrefabs = new GameObject[] { eggplatePrefab, omuricePrefab, parfaitPrefab, sushiPrefab };
        Sprite[] allWantedSprites = { eggPlateWantedSprite, omuriceWantedSprite, parfaitWantedSprite, sushiWantedSprite };
        
        // 포스터 랜덤 선택
        targetFoodIndex = Random.Range(0, 4);
        
        // Wanted 포스터 표시
        wantedPosterFood.sprite = allWantedSprites[targetFoodIndex];
        
        // 접시 n개 선택
        List<int> selectedIndices = SelectThreePlatesWithTarget(targetFoodIndex);
        
        // 접시 위치 계산
        Vector3[] positions = CalculatePlatePositions(n);

        // 현재 레벨 확인
        int currentLevel = StageManager.Instance.GetDifficulty();
        
        // 접시 n개 소환
        for (int i = 0; i < n; i++)
        {
            int foodIndex = selectedIndices[i];
            GameObject prefab = allPrefabs[foodIndex];
            
            // 프리팹 인스턴스화
            GameObject plateObj = Instantiate(prefab, platesContainer);
            
            // 위치 설정 (Transform - World Space)
            Transform plateTransform = plateObj.transform;
            if (plateTransform != null)
            {
                plateTransform.localPosition = positions[i];
                
                // 레벨에 따라 다른 스케일 적용
                float scaleMultiplier = (currentLevel == 4) ? plateScaleMultiplier_Level4 : plateScaleMultiplier;
                plateTransform.localScale = Vector3.one * scaleMultiplier;
            }
            
            // PlateController 설정
            PlateController plateController = plateObj.GetComponent<PlateController>();
            if (plateController != null)
            {
                // 이 접시가 정답인지 확인 (foodIndex가 targetFoodIndex와 같으면 정답)
                plateController.SetCorrectPlate(foodIndex == targetFoodIndex);
                spawnedPlates.Add(plateController);
            }
        }
    }
    
    // 접시 개수(n)에 따라 위치를 동적으로 계산
    private Vector3[] CalculatePlatePositions(int count)
    {
        switch (count)
        {
            case 2:
                return platePositions_2;
            case 3:
                return platePositions_3;
            default:
                return platePositions_4;
        }
    }
    
    private List<int> SelectThreePlatesWithTarget(int targetIndex)
    {
        List<int> result = new List<int>();
        List<int> others = new List<int>();
        
        // targetIndex(정답인덱스)는 무조건 포함
        result.Add(targetIndex);
        
        // 나머지 3개 삽입
        for (int i = 0; i < 4; i++)
        {
            if (i != targetIndex)
            {
                others.Add(i);
            }
        }
        
        // 랜덤으로 n-1개 선택
        for (int i = 0; i < n-1; i++)
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
        // shuffleCount번 반복
        for (int i = 0; i < shuffleCount; i++)
        {
            // 랜덤으로 2개의 접시 인덱스 선택
            int index1 = Random.Range(0, spawnedPlates.Count);
            int index2 = Random.Range(0, spawnedPlates.Count);
            
            // 같은 접시가 선택되지 않도록
            while (index2 == index1)
            {
                index2 = Random.Range(0, spawnedPlates.Count);
            }
            
            // 두 접시 교환 애니메이션
            yield return StartCoroutine(SwapPlates(index1, index2));
        }
    }
    
    // 두 접시의 위치를 교환하는 애니메이션
    private IEnumerator SwapPlates(int index1, int index2)
    {
        PlateController plate1 = spawnedPlates[index1];
        PlateController plate2 = spawnedPlates[index2];
        
        Transform transform1 = plate1.transform;
        Transform transform2 = plate2.transform;
        
        Vector3 startPos1 = transform1.localPosition;
        Vector3 startPos2 = transform2.localPosition;
        
        float elapsed = 0f;
        
        // Lerp로 위치 교환
        while (elapsed < swapDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / swapDuration;
            float smoothT = t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
            
            transform1.localPosition = Vector3.Lerp(startPos1, startPos2, smoothT);
            transform2.localPosition = Vector3.Lerp(startPos2, startPos1, smoothT);
            
            yield return null;
        }
        
        // 최종 위치 정확하게 설정
        transform1.localPosition = startPos2;
        transform2.localPosition = startPos1;
        
        // 리스트에서도 순서 교환
        spawnedPlates[index1] = plate2;
        spawnedPlates[index2] = plate1;
    }
    
    // 플레이어가 접시 선택
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
    
    // 선택 처리
    private IEnumerator HandlePlateSelection(PlateController selectedPlate)
    {
        currentGameState = GameState.Revealing;
        
        // 선택 표시
        selectedPlate.ShowSelected();
        yield return new WaitForSeconds(0.7f);
        
        // 뚜껑 다시 올리기 (이미지 교체 포함)
        selectedPlate.RaiseLid();
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
    
    public void OnEnable()
    {
        OnStageEnded += OnStageEndedGimmik;
    }
    
    public void OnDisable()
    {
        OnStageEnded -= OnStageEndedGimmik;
    }
    
    void Start()
    {
        OnStageStart();
        AdjustPosterScale(); // 포스터 크기도 조정
        StartCoroutine(GameSequence(n));
    }
    
    // 포스터 크기를 화면 비율에 맞춰 조정
    private void AdjustPosterScale()
    {
        if (wantedPosterFood != null)
        {
            Transform posterTransform = wantedPosterFood.transform;
            if (posterTransform != null)
            {
                posterTransform.localScale = Vector3.one * posterScaleMultiplier;
            }
        }
    }
}
