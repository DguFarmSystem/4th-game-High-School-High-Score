using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DrumTouchManager : MonoBehaviour
{
    [Serializable]
    public class NoteEntry
    {
        public float time; // 시간(초)
        public DrumType type; // 악기 종류
        public NoteState state; // 노트 상태
        [NonSerialized] public bool judged = false; // 이미 판정되었는지 여부
    }

    public enum DrumType { HiHat = 0, Snare = 1, Bass = 2 }
    public enum NoteState { HitRequired, ShouldNotHit, None }

    [Header("타이밍")]
    [Tooltip("(사용 중지) 레벨 플래시 오프셋을 사용합니다.")]
    public bool timingDisabled = true;

    [Header("노트 (사용자 입력)")]
    // 현재 플레이에 사용되는 노트 배열 (선택된 레벨의 배열이 로드되어 사용됨)
    public NoteEntry[] notes;

    [Header("레벨 설정")]
    [Tooltip("사용할 레벨을 선택하세요 (1~4). 레벨을 바꾸면 해당 레벨의 노트가 로드됩니다.")]
    [Range(1,4)]
    public int level = 1; // inspector에서 1..4로 조절

    [Header("레벨별 노트 (직접 채우세요)")]
    public NoteEntry[] level1Notes;
    public NoteEntry[] level2Notes;
    public NoteEntry[] level3Notes;
    public NoteEntry[] level4Notes;

    [Header("패드 (인덱스: 0=하이햇,1=스네어,2=베이스)")]
    public Collider2D[] padColliders = new Collider2D[3];

    [Header("상태 오브젝트 (순간 활성화)")]
    // 각 악기(패드)별로 순간적으로 활성화할 이미지 오브젝트
    // 인덱스: 0=하이햇, 1=스네어, 2=베이스
    public GameObject[] hitObjects = new GameObject[3]; // 플레이어가 쳐야할 때 순간 활성화
    public GameObject[] countObjects = new GameObject[3]; // 자동 재생일 때 순간 활성화
    // none은 기본적으로 아무것도 활성화하지 않음

    [Header("레벨별 배경/타이틀 이미지")]
    [Tooltip("각 레벨에 맞춰 활성화할 이미지 오브젝트(인덱스 0 = 레벨1, 1 = 레벨2, ...) ")]
    public GameObject[] levelImages = new GameObject[4];

    [Header("커스텀 타이밍 이미지")]
    [Tooltip("설정한 시간에 켜졌다가 꺼질 이미지 오브젝트 배열 (인덱스 맞춤)")]
    public GameObject[] timedFlashImages;
    [Tooltip("각 이미지가 켜질 절대 시간(초) 배열. timedFlashImages와 길이 맞춤")] 
    public float[] timedFlashTimes;
    // 내부: 각 이미지의 꺼질 절대시간(CurrentTime 기준)
    private float[] _timedFlashOffUntil;

    [Header("플래시/표시 설정")]
    [Tooltip("이미지 오브젝트를 활성화할 시간(초)")]
    public float flashDuration = 0.12f;

    [Header("레벨별 플래시 오프셋 (초)")]
    [Tooltip("노트 시간에 대해 표시를 시작할 오프셋(초). 예: -0.2f는 노트 0.2초 전부터 표시를 시작")] 
    public float level1FlashStart = -0.06f;
    public float level2FlashStart = -0.06f;
    public float level3FlashStart = -0.06f;
    public float level4FlashStart = -0.06f;

    [Tooltip("노트 시간에 대해 표시를 종료할 오프셋(초). 예: 0.12f는 노트 0.12초 후에 사라짐")]
    public float level1FlashEnd = 0.12f;
    public float level2FlashEnd = 0.12f;
    public float level3FlashEnd = 0.12f;
    public float level4FlashEnd = 0.12f;

    [Header("오디오 클립")]
    public AudioClip hiHatClip;
    public AudioClip snareClip;
    public AudioClip bassClip;

    // 곡/타이밍 소스는 사용하지 않음. 현재 시간은 레벨 시작 후 경과 시간으로 계산됩니다.

    private AudioSource _sfxSource;
    private float _lastTime = 0f;
    // 수동 플래시를 위한 타임스탬프(절대 시간, CurrentTime 기준)
    private float[] _manualHitUntil;
    private float[] _manualCountUntil;

    private void Awake()
    {
        _sfxSource = GetComponent<AudioSource>();
        if (_sfxSource == null) _sfxSource = gameObject.AddComponent<AudioSource>();
        // notes는 선택된 레벨에서 로드
        LoadLevelNotes();

        // 초기화: hit/count 오브젝트는 비활성화
        if (hitObjects != null)
            foreach (var o in hitObjects) if (o != null) o.SetActive(false);
        if (countObjects != null)
            foreach (var o in countObjects) if (o != null) o.SetActive(false);

        // 수동 플래시 타임스탬프 초기화
        _manualHitUntil = new float[Mathf.Max(3, hitObjects != null ? hitObjects.Length : 3)];
        _manualCountUntil = new float[Mathf.Max(3, countObjects != null ? countObjects.Length : 3)];

        // 레벨 이미지 초기화: 모든 이미지 비활성화 후 현재 level에 맞는 이미지 활성화
        if (levelImages != null)
        {
            for (int i = 0; i < levelImages.Length; i++)
            {
                if (levelImages[i] != null)
                    levelImages[i].SetActive(false);
            }
        }
        // timedFlashImages 초기화: offUntil 배열 생성 및 모든 이미지 비활성화
        if (timedFlashImages != null)
        {
            _timedFlashOffUntil = new float[timedFlashImages.Length];
            for (int i = 0; i < timedFlashImages.Length; i++)
            {
                if (timedFlashImages[i] != null) timedFlashImages[i].SetActive(false);
                _timedFlashOffUntil[i] = -1f;
            }
        }
        else
        {
            _timedFlashOffUntil = new float[0];
        }
        UpdateLevelImageActivation();
    }

    private void Start()
    {
        // InputManager의 탭 이벤트를 구독 (InputManager는 수정하지 않음)
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnStageTapPerformed += OnTap;
        }
        // 플레이 시작 시 선택된 레벨의 노트를 로드
        LoadLevelNotes();
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnStageTapPerformed -= OnTap;
        }
    }

    // 레벨에서 notes와 song을 로드하고 판정 플래그 초기화
    private void LoadLevelNotes()
    {
        // 노트는 선택된 레벨에서 로드
        switch (level)
        {
            case 1:
                notes = level1Notes;
                break;
            case 2:
                notes = level2Notes;
                break;
            case 3:
                notes = level3Notes;
                break;
            case 4:
                notes = level4Notes;
                break;
            default:
                notes = null;
                break;
        }

        // notes 정렬 및 judged 초기화
        if (notes != null)
        {
            Array.Sort(notes, (a, b) => a.time.CompareTo(b.time));
            foreach (var n in notes)
            {
                n.judged = false;
            }
        }
    }

    private float CurrentTime
    {
        get
        {
            // 현재는 song/audio 타이밍 소스를 사용하지 않습니다.
            // 레벨 시작 후 경과 시간을 현재 시간으로 사용합니다.
            return Time.timeSinceLevelLoad;
        }
    }

    // 에디터에서 레벨 값을 변경할 때 즉시 적용되도록 함
    private void OnValidate()
    {
        // 에디터에서만 동작: 레벨 범위를 안전하게 맞추고 노트로드
        if (!Application.isPlaying)
        {
            level = Mathf.Clamp(level, 1, 4); // level은 1~4로 제한
            // 노트는 에디터에서 바로 덮어씌우지 않음 — 다만 levels가 채워졌으면 인스펙터에서 확인 가능
            // 에디터에서 레벨을 바꿀 때 levelImages도 업데이트
            UpdateLevelImageActivation();
        }
    }

    // 런타임에서 레벨을 바꿀 때 사용할 수 있는 공개 메서드
    public void SetLevel(int newLevel)
    {
        level = Mathf.Clamp(newLevel, 1, 4);
        LoadLevelNotes();
        UpdateLevelImageActivation();
    }

    // 레벨에 따라 levelImages 배열의 오브젝트 활성화 처리
    private void UpdateLevelImageActivation()
    {
        if (levelImages == null) return;
        for (int i = 0; i < levelImages.Length; i++)
        {
            var obj = levelImages[i];
            if (obj == null) continue;
            obj.SetActive(i == (level - 1));
        }
    }

    private void Update()
    {
        if (notes == null || notes.Length == 0) return;

        float t = CurrentTime;

        // 1) 레벨 설정에 따른 start/end 오프셋을 가져옴
        (float startOffset, float endOffset) = GetLevelFlashOffsets();

        // 2) 자동 재생: count(치지 않는) 노트가 표시 창에 들어오면 자동으로 재생하고 판정 처리
        var toAuto = notes.Where(n => !n.judged && n.state == NoteState.ShouldNotHit && t >= n.time + startOffset && t < n.time + endOffset).ToArray();
        foreach (var an in toAuto)
        {
            an.judged = true;
            PlayClipForType(an.type);
            // 시각 피드백도 즉시 보이도록 수동 타임스탬프를 설정 (남은 창 길이)
            int padIdx = (int)an.type;
            float remaining = (an.time + endOffset) - t;
            float dur = remaining > 0f ? remaining : flashDuration;
            if (_manualCountUntil != null && padIdx >= 0 && padIdx < _manualCountUntil.Length)
                _manualCountUntil[padIdx] = Mathf.Max(_manualCountUntil[padIdx], t + dur);
        }

        // 각 패드별로 현재 시간 기준으로 표시 창 안에 있는 노트가 있는지 검사하여 오브젝트 활성화
        for (int pad = 0; pad < hitObjects.Length; pad++)
        {
            bool windowHit = notes.Any(n => n.type == (DrumType)pad && n.state == NoteState.HitRequired && t >= n.time + startOffset && t < n.time + endOffset);
            bool windowCount = notes.Any(n => n.type == (DrumType)pad && n.state == NoteState.ShouldNotHit && t >= n.time + startOffset && t < n.time + endOffset);

            bool hitActive = windowHit || ( _manualHitUntil != null && pad < _manualHitUntil.Length && t < _manualHitUntil[pad]);
            bool countActive = windowCount || ( _manualCountUntil != null && pad < _manualCountUntil.Length && t < _manualCountUntil[pad]);

            if (hitObjects != null && pad < hitObjects.Length && hitObjects[pad] != null)
                hitObjects[pad].SetActive(hitActive);
            if (countObjects != null && pad < countObjects.Length && countObjects[pad] != null)
                countObjects[pad].SetActive(countActive);
        }

        // timedFlashImages: 지정된 절대 시간에 flashDuration만큼 켰다가 끄기
        if (timedFlashImages != null && timedFlashTimes != null)
        {
            for (int i = 0; i < timedFlashImages.Length && i < timedFlashTimes.Length && i < _timedFlashOffUntil.Length; i++)
            {
                var img = timedFlashImages[i];
                if (img == null) continue;

                // 아직 꺼질 시간 미설정이고 현재 시간이 켜질 시간을 지났으면 켜고 offUntil 설정
                if (_timedFlashOffUntil[i] < 0f && t >= timedFlashTimes[i])
                {
                    img.SetActive(true);
                    _timedFlashOffUntil[i] = timedFlashTimes[i] + flashDuration;
                }

                // 꺼질 시간이 지나면 끔
                if (_timedFlashOffUntil[i] >= 0f && t >= _timedFlashOffUntil[i])
                {
                    img.SetActive(false);
                    _timedFlashOffUntil[i] = -1f; // 마커 리셋
                }
            }
        }

        // 3) 노트 만료 처리: 레벨 플래시 종료 시점을 지나면 미판정 노트를 마킹(미스)
        var expired = notes.Where(n => !n.judged && t >= n.time + endOffset).ToArray();
        foreach (var e in expired)
        {
            e.judged = true; // missed / expired
        }

        _lastTime = t;
    }

    private void OnTap()
    {
        if (InputManager.Instance == null) return;
        Vector3 touchPos = InputManager.Instance.TouchWorldPos;
        float t = CurrentTime;

        // 어떤 패드가 눌렸는지 검사
        for (int i = 0; i < padColliders.Length; i++)
        {
            var col = padColliders[i];
            if (col == null) continue;
            if (col.OverlapPoint(touchPos))
            {
                HandlePadTap(i, t);
                break; // 한 번의 탭에 하나의 패드만 처리
            }
        }
    }

    private void HandlePadTap(int padIndex, float currentTime)
    {
        // 해당 패드에서 판정되지 않은 노트 중 판정 윈도우 안에 있는 가장 가까운 노트를 찾음
        var (start, end) = GetLevelFlashOffsets();
        var candidate = notes
            .Where(n => !n.judged && (int)n.type == padIndex && currentTime >= n.time + start && currentTime < n.time + end)
            .OrderBy(n => Math.Abs(n.time - currentTime))
            .FirstOrDefault();

        if (candidate != null)
        {
            // 판정 처리
            candidate.judged = true;

            if (candidate.state == NoteState.HitRequired)
            {
                PlayClipForType(candidate.type);
                // 노트의 남은 창 길이를 계산해 그 길이만큼 수동 플래시 (없으면 flashDuration)
                var (startOffset2, endOffset2) = GetLevelFlashOffsets();
                float remaining = (candidate.time + endOffset2) - currentTime;
                float dur = remaining > 0f ? remaining : flashDuration;
                if (_manualHitUntil != null && padIndex >= 0 && padIndex < _manualHitUntil.Length)
                    _manualHitUntil[padIndex] = Mathf.Max(_manualHitUntil[padIndex], currentTime + dur);
            }
            else if (candidate.state == NoteState.ShouldNotHit)
            {
                // 치면 안되는 노트를 쳤을 때: 간단한 피드백 (count 오브젝트)
                var (s2, e2) = GetLevelFlashOffsets();
                float remaining2 = (candidate.time + e2) - currentTime;
                float dur2 = remaining2 > 0f ? remaining2 : flashDuration;
                if (_manualCountUntil != null && padIndex >= 0 && padIndex < _manualCountUntil.Length)
                    _manualCountUntil[padIndex] = Mathf.Max(_manualCountUntil[padIndex], currentTime + dur2);
            }
            else
            {
                // None — 시각 피드백 없음
            }
        }
        else
        {
            // 판정 가능한 노트 없음 (빈 탭): 간단한 피드백 (수동 타임스탬프)
            if (_manualCountUntil != null && padIndex >= 0 && padIndex < _manualCountUntil.Length)
                _manualCountUntil[padIndex] = Mathf.Max(_manualCountUntil[padIndex], currentTime + flashDuration);
        }
    }

    // ActivateObject 코루틴 제거 — 수동 타임스탬프 기반으로 통합되었습니다.

    private void PlayClipForType(DrumType type)
    {
        AudioClip clip = null;
        switch (type)
        {
            case DrumType.HiHat: clip = hiHatClip; break;
            case DrumType.Snare: clip = snareClip; break;
            case DrumType.Bass: clip = bassClip; break;
        }
        if (clip != null && _sfxSource != null)
        {
            _sfxSource.PlayOneShot(clip);
        }
    }

    // 현재 선택된 레벨에 맞는 플래시 start/end 오프셋을 반환
    private (float, float) GetLevelFlashOffsets()
    {
        switch (level)
        {
            case 1: return (level1FlashStart, level1FlashEnd);
            case 2: return (level2FlashStart, level2FlashEnd);
            case 3: return (level3FlashStart, level3FlashEnd);
            case 4: return (level4FlashStart, level4FlashEnd);
            default: return (level1FlashStart, level1FlashEnd);
        }
    }

    // (스프라이트 기반 유틸리티는 더 이상 사용되지 않습니다)
}
