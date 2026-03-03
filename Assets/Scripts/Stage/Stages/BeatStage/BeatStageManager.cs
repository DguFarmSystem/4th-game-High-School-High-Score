using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage;
using System.Linq;

public enum NoteType { Hi, Snare, Bass }

public class BeatStageManager : MonoBehaviour, IStageBase
{
    [Header("시간 기반 판정 설정")]
    public float StartDelay = 2.3f;
    public float FailDuration = 1.0f;
    [Tooltip("타이밍 오차 허용 범위 (예: +- 0.2초)")]
    public float HitTimeMargin = 0.2f;

    [Header("바 이동 속도 계산용 (UI 기준)")]
    [Tooltip("첫 번째 음표 UI를 넣으세요 (시작 위치 계산용)")]
    public RectTransform ReferencePoint1;
    [Tooltip("두 번째 음표 UI를 넣으세요 (속도 계산용)")]
    public RectTransform ReferencePoint2;
    [Tooltip("첫 번째 음표를 치는 시간 및 두 음표 사이의 시간 간격 (초)")]
    public float TimeBetweenReferencePoints = 1.0f;

    [Header("오브젝트 연결")]
    public GameObject ScanLineBar;

    [System.Serializable]
    public class NotePattern
    {
        public float[] HiTimes;
        public float[] SnareTimes;
        public float[] BassTimes;
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
    public AudioClip bassClip;

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
        if (ReferencePoint1 != null && ReferencePoint2 != null && TimeBetweenReferencePoints > 0f)
        {
            // 두 기준점 사이의 거리(X 좌표)와 걸리는 시간을 이용해 1초당 이동해야 할 픽셀(=속도)을 구합니다
            float distance = ReferencePoint2.position.x - ReferencePoint1.position.x;
            uiSpeedX = distance / TimeBetweenReferencePoints;

            // 0초일 때 스캔 라인 바가 위치해야 할 최초 시작 기준점 X위치를 역산해서 구합니다
            startPosX = ReferencePoint1.position.x - (uiSpeedX * TimeBetweenReferencePoints);
        }
        else
        {
            Debug.LogWarning("속도 계산 기준점(Reference) 1, 2가 비어있거나 시간이 0입니다! 게임이 멈춥니다.");
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

        if (ScanLineBar) ScanLineBar.SetActive(true);
        if (Level1_Sheet) Level1_Sheet.SetActive(true);

        yield return StartCoroutine(PlayRound(Level1_Pattern, Level1_Pattern.BassTimes));

        // [레벨 1] 정리
        if (ScanLineBar) ScanLineBar.SetActive(false);
        if (Level1_Sheet) Level1_Sheet.SetActive(false);

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

        if (ScanLineBar) ScanLineBar.SetActive(true);
        if (Level2_Sheet) Level2_Sheet.SetActive(true);

        // 레벨 2에서도 레벨 1의 둥둥 치는 베이스 타임을 강제로 가져와 똑같이 사용합니다.
        yield return StartCoroutine(PlayRound(Level2_Pattern, Level1_Pattern.BassTimes));

        // [레벨 2] 정리
        if (ScanLineBar) ScanLineBar.SetActive(false);
        if (Level2_Sheet) Level2_Sheet.SetActive(false);

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

    IEnumerator PlayRound(NotePattern pattern, float[] overrideBassTimes = null)
    {
        CalculateSpeedAndStart();

        if (ScanLineBar)
        {
            ScanLineBar.transform.position = new Vector3(startPosX, ScanLineBar.transform.position.y, ScanLineBar.transform.position.z);
        }

        isRoundFail = false;
        isGameRunning = true;

        SetupMemoryNotes(pattern, overrideBassTimes);
        OnStageStart();

        // 마지막 음표 시간 찾기
        float maxTime = 0f;
        foreach (var note in currentActiveNotes)
        {
            if (note.HitTime > maxTime) maxTime = note.HitTime;
        }

        // 마지막 음표를 친 후 "한 음표 간격"만큼 기다린 시점을 스테이지 종료점(EndPoint)으로 자동 계산
        float extraWait = (TimeBetweenReferencePoints > 0f) ? TimeBetweenReferencePoints : 1.0f;
        float endTime = maxTime + extraWait;

        while (currentStageTime < endTime)
        {
            if (CurrentStageState != StageState.Playing) yield break;
            yield return null;
        }

        isGameRunning = false;
    }

    void SetupMemoryNotes(NotePattern pattern, float[] overrideBassTimes)
    {
        currentActiveNotes.Clear();

        // 각 타입별 "가상 노트(VirtualNote)" 메모리 상에 생성
        if (pattern.HiTimes != null)
        {
            foreach (float t in pattern.HiTimes) CreateVirtualNote(t - 0.1f, NoteType.Hi);
        }
        if (pattern.SnareTimes != null)
        {
            foreach (float t in pattern.SnareTimes) CreateVirtualNote(t - 0.1f, NoteType.Snare);
        }

        // 베이스 타이밍은 넘어온 override 배열을 최우선으로 적용합니다. (레벨 1과 통일)
        float[] ultimateBassTimes = (overrideBassTimes != null && overrideBassTimes.Length > 0) ? overrideBassTimes : pattern.BassTimes;

        if (ultimateBassTimes != null)
        {
            foreach (float t in ultimateBassTimes) CreateVirtualNote(t, NoteType.Bass);
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

            // Bass는 판정 없이 정확한 시간에 자동 쾅!
            if (note.type == NoteType.Bass)
            {
                if (diffT <= 0f) // 재생해야 할 시간이 됐거나 살짝 지났음
                {
                    PlayBass();
                    note.isProcessed = true;
                }
            }
            else
            {
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
    }

    public void RegisterHittableNote(VirtualNote note)
    {
        // Bass는 자동 재생이므로 큐에 넣지 않습니다.
        if (note.type == NoteType.Hi) hittableHiNotes.Enqueue(note);
        else if (note.type == NoteType.Snare) hittableSnareNotes.Enqueue(note);
    }

    public void UnregisterNote(VirtualNote note, bool isMiss)
    {
        // Bass는 Miss 판정이 없으므로 무시
        if (note.type == NoteType.Bass) return;

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
    public void PlayBass() { if (audioSource && bassClip) audioSource.PlayOneShot(bassClip); }

    private void OnStageEndedGimmick(bool isClear) { StageManager.Instance.StageClear(isClear); }
    public void SetStageClear() { CurrentStageState = StageState.Clear; OnStageEnded?.Invoke(true); }
    public void SetStageFailed() { CurrentStageState = StageState.Over; OnStageEnded?.Invoke(false); }
}