using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage;

public class CountManager : StageNormal
{
    public GameObject Cat1;
    public GameObject Cat2;
    public GameObject Octo;
    public List<GameObject> spawnPoint; // 10개
    private int randomIdx;
    private int randomCat;
    private int animalCount;
    private int currentanimalCount;
    private int level = 1;
    public GameObject Manbo;
    private Clicker clicker;
    private float gapTime = 0f;
    private float spawnTimer = 0f;
    public float animalSpeed = 10f;
    [SerializeField] private float preparationTime = 2.3f; // 동물 나오기 전 대기 시간 (타이머)
    [SerializeField] private float endMargin = 2.0f;       // 마지막 동물 스폰 후 여유 시간
    [SerializeField] private ProcessTimer processTimer;    // 외부 타이머 오브젝트
    [SerializeField] private bool autoCalculateGap = false; // 기본값을 수동(랜덤)으로 변경
    [SerializeField] private float minSpawnInterval = 0.2f; // 최소 스폰 간격
    [SerializeField] private float maxSpawnInterval = 0.5f; // 최대 스폰 간격

    public void OnEnable()
    {
        OnStageEnded += OnStageEndedGimmik;
    }

    public void OnDisable()
    {
        OnStageEnded -= OnStageEndedGimmik;
    }

    private void OnStageEndedGimmik(bool isStageCleared)
    {
        if (isStageCleared)
        {
            Debug.Log("[CountStage] Stage cleared!");
            StageManager.Instance.StageClear(true);
        }
        else
        {
            Debug.Log("[CountStage] Stage failed!");
            StageManager.Instance.StageClear(false);
        }
    }

    void Awake()
    {
        // ProcessTimer는 Awake에서 미리 꺼야 Start()가 실행되지 않습니다.
        // 2.3초 대기 후 켜면 그때 Start()가 처음 호출되어 정확한 5초를 가져갑니다.
        if (processTimer != null)
        {
            processTimer.enabled = false;
        }
    }

    void Start()
    {
        // 씬 시작 시 바로 재생되는 StageNormal 기본 로직
        if (CurrentStageState == StageState.NotStart)
        {
            OnStageStart();
        }
    }

    public override void OnStageStart()
    {
        // 1. 매니저에서 난이도(레벨) 가져오기
        if (StageManager.Instance != null)
        {
            level = Mathf.Max(1, StageManager.Instance.GetDifficulty());
        }

        // 2. 레벨에 따른 정답 개수 부여
        if (level == 1 || level == 2)
            animalCount = Random.Range(3, 6);
        else
            animalCount = Random.Range(6, 11);

        // 3. 변수 및 클릭커 초기화
        clicker = Manbo.GetComponent<Clicker>();
        if (clicker != null)
        {
            clicker.countNum = 0;
            if (clicker.countText != null) clicker.countText.text = clicker.countNum.ToString("D5");
            // 게임 시작 전(카운트다운 중)에는 클릭 방지
            if (clicker.myButton != null) clicker.myButton.interactable = false;
        }

        currentanimalCount = 0;
        spawnTimer = 0f;

        // 4. 스폰 타이머 설정
        if (autoCalculateGap)
        {
            // 실제 동물이 나오는 시간(actionTime)은 타이머 시간에서 종료 여유시간을 뺀 만큼입니다.
            float actionTime = Mathf.Max(1f, timerTime - endMargin);
            gapTime = actionTime / animalCount;
        }
        else
        {
            // 첫 번째 스폰 간격을 랜덤하게 설정
            gapTime = Random.Range(minSpawnInterval, maxSpawnInterval);
        }

        // 5. 카운트다운을 위한 커루틴 시작 (여기서 2.3초 대기)
        StartCoroutine(PreparationSequence());
    }

    private IEnumerator PreparationSequence()
    {
        // 1. preparationTime(2.3초) 동안 대기
        float elapsed = 0f;
        while (elapsed < preparationTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 2. 대기 후 클릭 버튼 활성화
        if (clicker != null && clicker.myButton != null)
        {
            clicker.myButton.interactable = true;
        }

        // 3. 타이머 스크립트 활성화 (이제부터 시간이 줄어듭니다)
        if (processTimer != null)
        {
            processTimer.enabled = true;
            processTimer.StartTimer(); // 다시 시작하여 0부터 5초까지 계산하게 함
        }

        // 4. 부모 StageNormal 타이머 시작
        base.OnStageStart();
    }

    void Update()
    {
        // 준비 시간은 위에서 코루틴으로 처리하므로 여기서는 바로 Playing 상태 체크만 합니다.
        if (CurrentStageState != StageState.Playing) return;

        // 동물 스폰 타이머 로직
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= gapTime && currentanimalCount < animalCount)
        {
            Spawn(level);

            spawnTimer = 0f;
            currentanimalCount++;

            // 수동(랜덤) 모드일 경우, 다음 동물이 나올 간격을 다시 랜덤하게 결정
            if (!autoCalculateGap)
            {
                gapTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            }
        }
    }

    // StageNormal의 timerTime이 끝나면 자동으로 호출됩니다.
    protected override void OnStageEnd()
    {
        // 시간 종료 시, 클릭 수와 정답 수를 최종 비교하여 클리어/실패 여부 변경
        if (clicker != null && clicker.countNum == animalCount)
        {
            CurrentStageState = StageState.Clear; // 클리어 판정
            Debug.Log($"게임 성공 판단! (정답: {animalCount}, 입력: {clicker.countNum})");
        }
        else
        {
            CurrentStageState = StageState.Over; // 실패 판정
            int inputNum = clicker != null ? clicker.countNum : 0;
            Debug.Log($"게임 실패 판단... (정답: {animalCount}, 입력: {inputNum})");
        }

        // OnStageEndedGimmik 에 이벤트를 날려 StageManager 연동
        base.OnStageEnd();
    }
    void Spawn(int level)
    {
        randomIdx = Random.Range(0, spawnPoint.Count);
        randomCat = Random.Range(0, 2);

        // 스폰 지점(부모 캔버스 패널이나 빈 오브젝트) 가져오기
        Transform parentTransform = spawnPoint[randomIdx].transform;

        GameObject spawnedAnimal = null;
        if (level == 1 || level == 3) // 고양이
        {
            if (randomCat == 0)
            {
                spawnedAnimal = Instantiate(Cat1, parentTransform);
            }
            else
            {
                spawnedAnimal = Instantiate(Cat2, parentTransform);
            }
        }
        else // 문어
        {
            spawnedAnimal = Instantiate(Octo, parentTransform);
        }

        // 스폰된 UI 이미지의 스케일이나 로컬 위치가 캔버스 밖으로 튀지 않도록 초기화 방어코드
        if (spawnedAnimal != null)
        {
            RectTransform rect = spawnedAnimal.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
            }
        }

        var rb = spawnedAnimal.GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.left * animalSpeed, ForceMode2D.Impulse);
    }
}
