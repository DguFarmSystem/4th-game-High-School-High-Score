using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;

public class RestaurantFindStage : StageNormal
{
    private int n; 
    private int shuffleCount; 
    private float totalShuffleDuration = 2f; 
    
    [Header("접시 프리팹")]
    [SerializeField] private GameObject eggplatePrefab;
    [SerializeField] private GameObject omuricePrefab;
    [SerializeField] private GameObject parfaitPrefab;
    [SerializeField] private GameObject sushiPrefab;
    
    [Header("접시, 포스터 위치")]
    [SerializeField] private Transform platesContainer; 
    [SerializeField] private SpriteRenderer wantedPosterFood; 
    
    [Header("접시 위치 설정")]
    [SerializeField] private Vector3[] platePositions_2; 
    [SerializeField] private Vector3[] platePositions_3; 
    [SerializeField] private Vector3[] platePositions_4; 
    
    [Header("포스터")]
    [SerializeField] private Sprite eggPlateWantedSprite; 
    [SerializeField] private Sprite omuriceWantedSprite; 
    [SerializeField] private Sprite parfaitWantedSprite; 
    [SerializeField] private Sprite sushiWantedSprite; 
    
    [Header("애니메이션 속도")]
    [SerializeField] private float showFoodDuration = 2f; 
    [SerializeField] private float lidCloseDuration = 0.5f; 
    private float swapDuration; 
    
    [Header("화면 비율 대응")]
    [SerializeField] private float plateScaleMultiplier = 3f; 
    [SerializeField] private float plateScaleMultiplier_Level4 = 1f; 
    [SerializeField] private float posterScaleMultiplier = 0.5f; 
    
    [Header("오디오 설정")]
    [SerializeField] private AudioSource audioSource; // 오디오 소스 컴포넌트 연결 필요
    [SerializeField] private AudioClip shuffleClip;   // 섞을 때 (Shuffling)
    [SerializeField] private AudioClip touchClip;     // 접시 터치 시 (Select)
    [SerializeField] private AudioClip lidOpenClip;   // 뚜껑 열릴 때 (Reveal)
    [SerializeField] private AudioClip successClip;   // 성공 시 (Clear)
    [SerializeField] private AudioClip failClip;      // 실패 시 (Fail)

    public float LidCloseDuration => lidCloseDuration; 
    
    private List<PlateController> spawnedPlates = new(); 
    private int targetFoodIndex; 
    
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
    
    public override void OnStageStart()
    {
        base.OnStageStart(); 
        CurrentStageState = StageState.Playing;
        
        // [Audio] AudioSource가 없으면 자동으로 가져오거나 추가 (안전장치)
        if (audioSource == null) 
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        }

        InitializeLevelSettings();
    }
    
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
        
        swapDuration = totalShuffleDuration / shuffleCount;
    }
    
    protected override void OnStageEnd() 
    {
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
        
        PlateController correctPlate = spawnedPlates.Find(plate => plate.IsCorrectPlate);
        
        if (correctPlate != null)
        {
            // [Audio] 뚜껑 열림 소리
            PlaySound(lidOpenClip);

            correctPlate.RaiseLid();
            yield return new WaitForSeconds(lidCloseDuration);
            
            // [Audio] 타임오버도 실패이므로 실패 소리 재생 (0.5초 후)
            PlaySound(failClip);
        }
        
        base.OnStageEnd();
        currentGameState = GameState.Ended;
    }

    protected override void OnStageClear() 
    {
        base.OnStageClear();
    }
    
    private void OnStageEndedGimmik(bool isStageCleared)
    {
        if (isStageCleared) 
        {
            Debug.Log("성공!");
            StageManager.Instance.StageClear(true);
        }
        else 
        {
            Debug.Log("실패!");
            StageManager.Instance.StageClear(false);
        }
    }
    
    private IEnumerator GameSequence(int n)
    {
        SpawnPlatesWithTarget(n);
        
        currentGameState = GameState.ShowingFood;
        yield return new WaitForSeconds(showFoodDuration);
        
        currentGameState = GameState.ClosingLid;
        yield return StartCoroutine(CloseLids());
        
        currentGameState = GameState.Shuffling;
        yield return StartCoroutine(ShufflePlates());
        
        currentGameState = GameState.WaitingSelection;
        canSelectPlate = true;
    }

    private void SpawnPlatesWithTarget(int n)
    {
        // (기존 코드와 동일)
        GameObject[] allPrefabs;
        allPrefabs = new GameObject[] { eggplatePrefab, omuricePrefab, parfaitPrefab, sushiPrefab };
        Sprite[] allWantedSprites = { eggPlateWantedSprite, omuriceWantedSprite, parfaitWantedSprite, sushiWantedSprite };
        
        targetFoodIndex = Random.Range(0, 4);
        wantedPosterFood.sprite = allWantedSprites[targetFoodIndex];
        
        List<int> selectedIndices = SelectThreePlatesWithTarget(targetFoodIndex);
        Vector3[] positions = CalculatePlatePositions(n);

        int currentLevel = StageManager.Instance.GetDifficulty();
        
        for (int i = 0; i < n; i++)
        {
            int foodIndex = selectedIndices[i];
            GameObject prefab = allPrefabs[foodIndex];
            GameObject plateObj = Instantiate(prefab, platesContainer);
            
            Transform plateTransform = plateObj.transform;
            if (plateTransform != null)
            {
                plateTransform.localPosition = positions[i];
                float scaleMultiplier = (currentLevel == 4) ? plateScaleMultiplier_Level4 : plateScaleMultiplier;
                plateTransform.localScale = Vector3.one * scaleMultiplier;
            }
            
            PlateController plateController = plateObj.GetComponent<PlateController>();
            if (plateController != null)
            {
                plateController.SetCorrectPlate(foodIndex == targetFoodIndex);
                spawnedPlates.Add(plateController);
            }
        }
    }
    private Vector3[] CalculatePlatePositions(int count)
    {
        switch (count)
        {
            case 2: return platePositions_2;
            case 3: return platePositions_3;
            default: return platePositions_4;
        }
    }

    private List<int> SelectThreePlatesWithTarget(int targetIndex)
    {
        List<int> result = new List<int>();
        List<int> others = new List<int>();
        result.Add(targetIndex);
        for (int i = 0; i < 4; i++) { if (i != targetIndex) others.Add(i); }
        for (int i = 0; i < n-1; i++) { int randomIndex = Random.Range(0, others.Count); result.Add(others[randomIndex]); others.RemoveAt(randomIndex); }
        for (int i = 0; i < result.Count; i++) { int temp = result[i]; int randomIndex = Random.Range(0, result.Count); result[i] = result[randomIndex]; result[randomIndex] = temp; }
        return result;
    }

    private IEnumerator CloseLids()
    {
        foreach (var plate in spawnedPlates) plate.CloseLid();
        yield return new WaitForSeconds(lidCloseDuration);
    }

    // 섞기
    private IEnumerator ShufflePlates()
    {
        for (int i = 0; i < shuffleCount; i++)
        {
            // [Audio] 섞기 시작할 때 소리 재생
            PlaySound(shuffleClip);

            int index1 = Random.Range(0, spawnedPlates.Count);
            int index2 = Random.Range(0, spawnedPlates.Count);
            
            while (index2 == index1)
            {
                index2 = Random.Range(0, spawnedPlates.Count);
            }
            
            yield return StartCoroutine(SwapPlates(index1, index2));
        }
    }
    
    private IEnumerator SwapPlates(int index1, int index2)
    {
        PlateController plate1 = spawnedPlates[index1];
        PlateController plate2 = spawnedPlates[index2];
        
        Transform transform1 = plate1.transform;
        Transform transform2 = plate2.transform;
        
        Vector3 startPos1 = transform1.localPosition;
        Vector3 startPos2 = transform2.localPosition;
        
        float elapsed = 0f;
        
        while (elapsed < swapDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / swapDuration;
            float smoothT = t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
            
            transform1.localPosition = Vector3.Lerp(startPos1, startPos2, smoothT);
            transform2.localPosition = Vector3.Lerp(startPos2, startPos1, smoothT);
            
            yield return null;
        }
        
        transform1.localPosition = startPos2;
        transform2.localPosition = startPos1;
        
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

        // 접시 터치(선택) 소리 재생
        PlaySound(touchClip);
        
        StartCoroutine(HandlePlateSelection(selectedPlate));
    }
    
    // 선택 처리
    private IEnumerator HandlePlateSelection(PlateController selectedPlate)
    {
        currentGameState = GameState.Revealing;
        
        selectedPlate.ShowSelected();
        yield return new WaitForSeconds(0.7f);
        
        // 뚜껑 열림 소리 재생 (RaiseLid 직전)
        PlaySound(lidOpenClip);

        selectedPlate.RaiseLid();
        
        // 뚜껑이 열리는 애니메이션 시간만큼 대기 (lidCloseDuration = 0.5초 등)
        yield return new WaitForSeconds(lidCloseDuration);
        
        // 뚜껑이 완전히 열린 후(0.5초 뒤) 결과에 따라 소리 재생
        if (selectedPlate.IsCorrectPlate)
        {
            PlaySound(successClip); // 성공 사운드
            OnStageClear();
        }
        else
        {
            PlaySound(failClip); // 실패 사운드
            CurrentStageState = StageState.Over;
            OnStageEnd();
        }
        
        currentGameState = GameState.Ended;
    }
    
    // 소리 재생 헬퍼 함수
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
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
        AdjustPosterScale(); 
        StartCoroutine(GameSequence(n));
    }
    
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