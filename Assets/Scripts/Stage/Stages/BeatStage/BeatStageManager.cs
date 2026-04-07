using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage;
using System.Linq;

public enum NoteType { Hi, Snare }

[System.Serializable]
public struct NoteSlot
{
    public bool isHi;
    public bool isSnare;
}

public class BeatStageManager : MonoBehaviour, IStageBase
{
    [Header("시간 기반 판정 설정")]
    public float StartDelay = 2.3f;
    public float FailDuration = 1.0f;
    [Tooltip("타이밍 오차 허용 범위 (예: +- 0.2초)")]
    public float HitTimeMargin = 0.2f;

    [Header("바 이동 시작점 & 속도 계산용 (UI 기준)")]
    [Tooltip("캔버스상의 첫 번째 연속 음표 (1초 위치)\n*바(ScanLineBar) 자체의 현재 위치가 0초 출발점이 됩니다.")]
    public RectTransform ReferencePoint1;
    [Tooltip("캔버스상의 두 번째 연속 음표 (2초 위치) - 속도 계산용")]
    public RectTransform ReferencePoint2;

    [Header("오브젝트 연결")]
    public GameObject ScanLineBar;

    [System.Serializable]
    public class NotePattern
    {
        [Tooltip("0초부터 9초까지 각 타이밍(총 10개)에 하이와 스네어의 포함 여부를 결정합니다.\n배열 길이를 더 늘리면 스테이지 유지 길이도 맞춰서 길어집니다.")]
        public NoteSlot[] Notes = new NoteSlot[10];
    }

    [Header("레벨 1 설정")]
    public GameObject Level1_Sheet;
    public NotePattern Level1_Pattern;

    [Header("레벨 2 설정")]
    public GameObject Level2_Sheet;
    public NotePattern Level2_Pattern;

    [Header("이펙트 & 사운드")]
    public GameObject HiHitEffect;
    public GameObject SnareHitEffect;
    public GameObject HiFailArt;
    public GameObject SnareFailArt;

    [Header("UI 드럼 연결(판정용)")]
    public RectTransform HiDrumUI;
    public RectTransform SnareDrumUI;

    [Header("실패 연출 위치 (Canvas 기준)")]
    public RectTransform HiFailPosition;
    public RectTransform SnareFailPosition;

    [Header("오디오 클립")]
    public AudioClip hiClip;
    public AudioClip snareClip;
    [Header("배경 베이스(드럼) 사운드")]
    public AudioClip level1BassClip;
    public AudioClip level2BassClip;

    private AudioClip currentBassClip;

    private AudioSource audioSource;

    // === 내부 변수 ===
    private bool isGameRunning = false;
    private bool isRoundFail = false;

    private float uiSpeedX;
    private float startPosX;
    private float currentStageTime;

    // 메모리 상에서 판정만 담당할 가상 노트
    public class VirtualNote
    {
        public NoteType type;
        public float HitTime;
        public bool isProcessed = false;
        public bool isRegistered = false;
    }

    private Queue<VirtualNote> hittableHiNotes = new();
    private Queue<VirtualNote> hittableSnareNotes = new();

    private Coroutine hiFailRoutine;
    private Coroutine snareFailRoutine;

    private List<VirtualNote> currentActiveNotes = new List<VirtualNote>();

    // Interface
    public StageState CurrentState => CurrentStageState;
    protected StageState CurrentStageState = StageState.NotStart;
    public Action<bool> OnStageEnded { get; protected set; }
    public float StageTimeLimit { get; private set; } = 0f;

    public void OnEnable() => OnStageEnded += OnStageEndedGimmick;
    public void OnDisable() => OnStageEnded -= OnStageEndedGimmick;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        if (HiFailArt) HiFailArt.SetActive(false);
        if (SnareFailArt) SnareFailArt.SetActive(false);

        if (HiHitEffect) HiHitEffect.SetActive(false);
        if (SnareHitEffect) SnareHitEffect.SetActive(false);

        if (ScanLineBar) ScanLineBar.SetActive(false);

        if (Level1_Sheet) Level1_Sheet.SetActive(false);
        if (Level2_Sheet) Level2_Sheet.SetActive(false);
    }

    void CalculateSpeedAndStart()
    {
        if (ReferencePoint1 != null && ReferencePoint2 != null && ScanLineBar != null)
        {
            // 속도는 연속된 두 음표 간의 거리로 도출합니다. (매 1초마다 이동할 픽셀량)
            uiSpeedX = ReferencePoint2.position.x - ReferencePoint1.position.x;

            // 유저가 에디터에 배치해둔 바 객체의 현재 위치를 초기 시작점(0초)으로 고정
            startPosX = ScanLineBar.transform.position.x;
        }
        else
        {
            Debug.LogWarning("바 이동을 계산할 ReferencePoint 1, 2 또는 ScanLineBar가 비어있습니다! 바가 제자리에 멈출 수 있습니다.");
        }
    }

    void Start()
    {
        StartCoroutine(StageRoutine());
    }

    IEnumerator StageRoutine()
    {
        yield return new WaitForSeconds(StartDelay);

        Debug.Log("레벨 1 시작");
        currentBassClip = level1BassClip;
        if (audioSource && currentBassClip) { audioSource.clip = currentBassClip; audioSource.PlayDelayed(1.0f); }

        if (ScanLineBar) ScanLineBar.SetActive(true);
        if (Level1_Sheet) Level1_Sheet.SetActive(true);

        yield return StartCoroutine(PlayRound(Level1_Pattern));

        // [레벨 1] 정리
        if (ScanLineBar) ScanLineBar.SetActive(false);
        if (Level1_Sheet) Level1_Sheet.SetActive(false);
        if (audioSource) audioSource.Stop();

        hittableHiNotes.Clear();
        hittableSnareNotes.Clear();

        // [레벨 1] 결과 판정
        if (isRoundFail)
        {
            Debug.Log("레벨 1 실패 -> 게임 오버");
            SetStageFailed();
            yield break;
        }

        yield return new WaitForSeconds(StartDelay);

        Debug.Log("레벨 2 시작");
        currentBassClip = level2BassClip;
        if (audioSource && currentBassClip) { audioSource.clip = currentBassClip; audioSource.PlayDelayed(1.0f); }

        if (ScanLineBar) ScanLineBar.SetActive(true);
        if (Level2_Sheet) Level2_Sheet.SetActive(true);

        yield return StartCoroutine(PlayRound(Level2_Pattern));

        // [레벨 2] 정리
        if (ScanLineBar) ScanLineBar.SetActive(false);
        if (Level2_Sheet) Level2_Sheet.SetActive(false);
        if (audioSource) audioSource.Stop();

        // [레벨 2] 결과 판정
        if (isRoundFail)
        {
            Debug.Log("레벨 2 실패 -> 게임 오버");
            SetStageFailed();
        }
        else
        {
            Debug.Log("모든 스테이지 클리어");
            SetStageClear();
        }
    }

    IEnumerator PlayRound(NotePattern pattern)
    {
        CalculateSpeedAndStart();

        if (ScanLineBar)
        {
            ScanLineBar.transform.position = new Vector3(startPosX, ScanLineBar.transform.position.y, ScanLineBar.transform.position.z);
        }

        isRoundFail = false;
        isGameRunning = true;

        SetupMemoryNotes(pattern);
        OnStageStart();

        // 음표를 일찍 끝내도 레벨이 빨리 끝나지 않도록 배열 Length 기준으로 종료 시점을 고정합니다.
        // 예를 들어 10칸(9초)짜리면 마지막 판정이 10.0s 위치이고, 그 후 1초 대기하여 11.0s에 깔끔하게 종료됩니다.
        float maxTime = pattern.Notes.Length; 
        float extraWait = 1.0f;
        float endTime = maxTime + extraWait;

        while (currentStageTime < endTime)
        {
            if (CurrentStageState != StageState.Playing) yield break;
            yield return null;
        }

        isGameRunning = false;
    }

    void SetupMemoryNotes(NotePattern pattern)
    {
        currentActiveNotes.Clear();

        if (pattern.Notes != null)
        {
            for (int i = 0; i < pattern.Notes.Length; i++)
            {
                // 인덱스 0이 설정상 0초의 의미지만 1초 뒤에 재생되어야 하므로 i + 1.0f 를 사용합니다.
                if (pattern.Notes[i].isHi) CreateVirtualNote((float)i + 1.0f, NoteType.Hi);
                if (pattern.Notes[i].isSnare) CreateVirtualNote((float)i + 1.0f, NoteType.Snare);
            }
        }

        // 시간순으로 List 정렬 (스캔 로직 정확도를 위해)
        currentActiveNotes.Sort((a, b) => a.HitTime.CompareTo(b.HitTime));
    }

    void CreateVirtualNote(float time, NoteType type)
    {
        VirtualNote note = new VirtualNote
        {
            type = type,
            HitTime = time,
            isProcessed = false,
            isRegistered = false
        };
        currentActiveNotes.Add(note);
    }

    public void OnStageStart()
    {
        CurrentStageState = StageState.Playing;
        currentStageTime = 0f;
        isGameRunning = true;
    }

    void Update()
    {
        if (CurrentStageState != StageState.Playing) return;
        if (!isGameRunning) return;

        currentStageTime += Time.deltaTime;

        if (ScanLineBar)
        {
            // 시간에 따른 강제 좌표 이동 (위치는 이제 전적으로 '시간'에 의해 지배됩니다)
            ScanLineBar.transform.position = new Vector3(startPosX + (uiSpeedX * currentStageTime), ScanLineBar.transform.position.y, ScanLineBar.transform.position.z);
            CheckNotesCollision();
        }

        HandleInput();
    }

    void CheckNotesCollision()
    {
        if (!isGameRunning) return;

        // X 좌표 대신 오직 "시간차(Time)"로만 노트들을 판정합니다.
        foreach (var note in currentActiveNotes)
        {
            if (note.isProcessed) continue;

            float diffT = note.HitTime - currentStageTime;
            // diffT가 양수면 아직 칠 시간이 안 옴 (미래)
            // diffT가 음수면 칠 시간이 지났음 (과거)

            // 치기 전, 오차 범위(HitTimeMargin) 안으로 진입했다면 유저가 칠 수 있게 '등록'
            if (!note.isRegistered && Mathf.Abs(diffT) <= HitTimeMargin)
            {
                note.isRegistered = true;
                RegisterHittableNote(note);
            }
            // 유저가 안 치고 오차 범위(HitTimeMargin)를 완전히 통과해 버렸다면 'Miss'
            else if (note.isRegistered && diffT < -HitTimeMargin)
            {
                note.isProcessed = true;
                UnregisterNote(note, true); // (true) = Miss
            }
        }
    }

    public void RegisterHittableNote(VirtualNote note)
    {
        if (note.type == NoteType.Hi) hittableHiNotes.Enqueue(note);
        else if (note.type == NoteType.Snare) hittableSnareNotes.Enqueue(note);
    }

    public void UnregisterNote(VirtualNote note, bool isMiss)
    {
        if (note.type == NoteType.Hi)
        {
            if (hittableHiNotes.Count > 0 && hittableHiNotes.Peek() == note)
            {
                hittableHiNotes.Dequeue();
                if (isMiss)
                {
                    Debug.Log("Miss (Hi)");
                    ShowFailArt(true);
                    isRoundFail = true;
                }
            }
        }
        else if (note.type == NoteType.Snare)
        {
            if (hittableSnareNotes.Count > 0 && hittableSnareNotes.Peek() == note)
            {
                hittableSnareNotes.Dequeue();
                if (isMiss)
                {
                    Debug.Log("Miss (Snare)");
                    ShowFailArt(false);
                    isRoundFail = true;
                }
            }
        }
    }

    void HandleInput()
    {
        bool isHiActive = false;
        bool isSnareActive = false;

        // Screen Space - Camera 기반일 경우 Camera.main을, Overlay일 경우 null을 권장합니다.
        // 유니티 터치 좌표 버그를 해결하기 위해 null로 고정합니다.
        Camera uiCamera = Camera.main;

        // 1. 모바일 터치 처리
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            Vector2 touchPos = touch.position;

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if (HiDrumUI != null && RectTransformUtility.RectangleContainsScreenPoint(HiDrumUI, touchPos, uiCamera)) isHiActive = true;
                if (SnareDrumUI != null && RectTransformUtility.RectangleContainsScreenPoint(SnareDrumUI, touchPos, uiCamera)) isSnareActive = true;
            }

            if (touch.phase == TouchPhase.Began)
            {
                ProcessHit(touchPos, uiCamera);
            }
        }

        // 2. 유니티 에디터(PC 마우스) 처리
        if (Input.touchCount == 0)
        {
            Vector2 mousePos = Input.mousePosition;

            if (Input.GetMouseButton(0))
            {
                if (HiDrumUI != null && RectTransformUtility.RectangleContainsScreenPoint(HiDrumUI, mousePos, uiCamera)) isHiActive = true;
                if (SnareDrumUI != null && RectTransformUtility.RectangleContainsScreenPoint(SnareDrumUI, mousePos, uiCamera)) isSnareActive = true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                ProcessHit(mousePos, uiCamera);
            }
        }

        // 이펙트 껐다 켜기
        if (HiHitEffect)
        {
            if (HiDrumUI) HiHitEffect.transform.position = HiDrumUI.position;
            HiHitEffect.SetActive(isHiActive);
        }
        if (SnareHitEffect)
        {
            if (SnareDrumUI) SnareHitEffect.transform.position = SnareDrumUI.position;
            SnareHitEffect.SetActive(isSnareActive);
        }
    }

    // 터치나 마우스 클릭이 발생했을 때 처리하는 공통 로직
    void ProcessHit(Vector2 hitPos, Camera uiCamera)
    {
        bool hitHi = HiDrumUI != null && RectTransformUtility.RectangleContainsScreenPoint(HiDrumUI, hitPos, uiCamera);
        bool hitSnare = SnareDrumUI != null && RectTransformUtility.RectangleContainsScreenPoint(SnareDrumUI, hitPos, uiCamera);

        if (hitHi)
        {
            PlayHi();

            // 등록된 노트 중 맨 앞의 것을 친다
            if (hittableHiNotes.Count > 0)
            {
                VirtualNote note = hittableHiNotes.Dequeue();
                note.isProcessed = true;
                Debug.Log("Hi Hit Success!");
            }
            else
            {
                Debug.Log("Hi Fail (빈 공간 터치)");
                ShowFailArt(true);
            }
        }
        else if (hitSnare)
        {
            PlaySnare();

            if (hittableSnareNotes.Count > 0)
            {
                VirtualNote note = hittableSnareNotes.Dequeue();
                note.isProcessed = true;
                Debug.Log("Snare Hit Success!");
            }
            else
            {
                Debug.Log("Snare Fail (빈 공간 터치)");
                ShowFailArt(false);
            }
        }
    }

    void ShowFailArt(bool isHi)
    {
        GameObject target = isHi ? HiFailArt : SnareFailArt;
        if (target == null) return;

        // 지정된 실패 연출(Fail Position) UI 위치로 동기화
        if (isHi && HiFailPosition != null) target.transform.position = HiFailPosition.position;
        else if (!isHi && SnareFailPosition != null) target.transform.position = SnareFailPosition.position;

        if (isHi) { if (hiFailRoutine != null) StopCoroutine(hiFailRoutine); hiFailRoutine = StartCoroutine(FailRoutine(target)); }
        else { if (snareFailRoutine != null) StopCoroutine(snareFailRoutine); snareFailRoutine = StartCoroutine(FailRoutine(target)); }
    }

    IEnumerator FailRoutine(GameObject obj)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(FailDuration);
        obj.SetActive(false);
    }

    // === 사운드 재생 함수 ===
    public void PlayHi() { if (audioSource) audioSource.PlayOneShot(hiClip); }
    public void PlaySnare() { if (audioSource) audioSource.PlayOneShot(snareClip); }

    private void OnStageEndedGimmick(bool isClear) { StageManager.Instance.StageClear(isClear); }
    public void SetStageClear() { CurrentStageState = StageState.Clear; OnStageEnded?.Invoke(true); }
    public void SetStageFailed() { CurrentStageState = StageState.Over; OnStageEnded?.Invoke(false); }
}