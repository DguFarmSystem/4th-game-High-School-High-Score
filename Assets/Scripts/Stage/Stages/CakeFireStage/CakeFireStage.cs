// Assets/Scripts/Stage/Stages/CakeFireStage/CakeFireStage.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Stage
{
    [DisallowMultipleComponent]
    public class CakeFireStage : StageNormal
    {
        [System.Serializable]
        public class LevelEntry
        {
            public GameObject root;          // Cake_Lv1, Lv2...
            public SpriteRenderer dimmer;    // CakeDimmer (SpriteRenderer)
        }

        [Header("Levels (난이도별 케이크)")]
        [SerializeField] List<LevelEntry> levels = new List<LevelEntry>();

        [Header("Stage Rule")]
        [SerializeField] float timeLimit = 10f;

        [Header("Dimmer Settings")]
        [Range(0f, 1f)] [SerializeField] float maxDarkAlpha = 0.95f;
        [SerializeField] float dimmerLerpSpeed = 6f;

        [Header("Render Safety")]
        [SerializeField] int  dimmerSortingOrderMin = 2000;
        [SerializeField] bool useMaskVisibleInside = true;

        [Header("Clear Delay")]
        [Tooltip("마지막 초에 불 붙고 다음 레벨/스테이지로 넘어가기 전 연출 대기 시간")]
        [SerializeField] float clearDelayAfterLastLit = 0.6f;

        [Header("Dimmer Smoothing")]
        [SerializeField] float dimmerSmoothTime = 0.25f;
        [SerializeField] float dimmerMaxSpeed  = 10f;
        float _alphaVel;   

        int _activeIndex = -1;
        LevelEntry _active;
        readonly List<CandleFire> _candles = new();

        int _needCount;
        int _litCount;

        float _currentAlpha;
        float _targetAlpha;

        bool _clearPending;                  // 마지막 점화 후 지연 중 플래그
        Coroutine _levelTimerCo;             // 내부 레벨 타이머 핸들

        // ─────────────────────────────────────────────────────────────
        void OnEnable()  => OnStageEnded += OnStageEndedGimmick;
        void OnDisable() => OnStageEnded -= OnStageEndedGimmick;

        void Start()
        {
            if (CurrentStageState == StageState.NotStart)
                OnStageStart();
        }

        public override void OnStageStart()
        {
            int diff = 1;
            if (StageManager.Instance != null)
                diff = Mathf.Max(1, StageManager.Instance.GetDifficulty());

            _activeIndex = Mathf.Clamp(diff - 1, 0, Mathf.Max(0, levels.Count - 1));

            StopAllCoroutines();
            _levelTimerCo = null;
            _clearPending = false;
            CurrentStageState = StageState.Playing;

            SetupLevel(_activeIndex);
            _levelTimerCo = StartCoroutine(LevelTimer(timeLimit));
        }

        protected override void OnStageEnd()
        {
            foreach (var c in _candles) c.OnIgnited -= HandleIgnite;
            base.OnStageEnd();
        }

        void Update()
        {
            if (!Mathf.Approximately(_currentAlpha, _targetAlpha))
            {
                _currentAlpha = Mathf.SmoothDamp(
                    _currentAlpha,
                    _targetAlpha,
                    ref _alphaVel,
                    dimmerSmoothTime,
                    dimmerMaxSpeed <= 0f ? Mathf.Infinity : dimmerMaxSpeed,
                    Time.deltaTime
                );

                ApplyDimmerImmediate(_currentAlpha);
            }
        }

        // 내부 레벨

        void SetupLevel(int index)
        {
            for (int i = 0; i < levels.Count; i++)
                if (levels[i].root) levels[i].root.SetActive(i == index);

            _active = (index >= 0 && index < levels.Count) ? levels[index] : null;

            WireAndFixDimmer();
            _currentAlpha = maxDarkAlpha;
            _targetAlpha  = maxDarkAlpha;
            ApplyDimmerImmediate(_currentAlpha, ensureEnabled: true);

            foreach (var c in _candles) if (c != null) c.OnIgnited -= HandleIgnite;
            _candles.Clear();
            if (_active != null && _active.root != null)
                _candles.AddRange(_active.root.GetComponentsInChildren<CandleFire>(true));
            foreach (var c in _candles) c.OnIgnited += HandleIgnite;

            _clearPending = false;
            RefreshCounts();
            UpdateTargetAlpha();
            ApplyDimmerImmediate(_targetAlpha, ensureEnabled: true);
        }

        void GoToNextInternalLevel()
        {
            if (_activeIndex + 1 < levels.Count)
            {
                _activeIndex++;
                Debug.Log($"[CakeFireStage] ► Next Level: Lv{_activeIndex + 1}");

                StopAllCoroutines();
                _levelTimerCo = null;
                _clearPending = false;
                CurrentStageState = StageState.Playing;

                SetupLevel(_activeIndex);
                _levelTimerCo = StartCoroutine(LevelTimer(timeLimit));
                return;
            }

            // 마지막 레벨 완료 → 스테이지 클리어
            Debug.Log("[CakeFireStage] ★ Stage Cleared (all internal levels)!");
            CurrentStageState = StageState.Clear;

            StopAllCoroutines();
            StartCoroutine(EndAfter(0.25f));
        }

        System.Collections.IEnumerator EndAfter(float t)
        {
            yield return new WaitForSeconds(t);
            OnStageEnd();
        }

        System.Collections.IEnumerator LevelTimer(float seconds)
        {
            float t = seconds;
            while (t > 0f && CurrentStageState == StageState.Playing && !_clearPending)
            {
                t -= Time.deltaTime;
                yield return null;
            }
            if (CurrentStageState == StageState.Playing && !_clearPending)
            {
                CurrentStageState = StageState.Over;
                OnStageEnd();
            }
        }

        // 점화/밝기
        void HandleIgnite(CandleFire _)
        {
            RefreshCounts();

            UpdateTargetAlpha();

            Debug.Log($"[CakeFireStage] ignite handled: {_litCount}/{_needCount}");

            if (_needCount > 0 && _litCount >= _needCount)
            {
                Debug.Log($"[CakeFireStage] ✓ Level {(_activeIndex + 1)} Cleared!");
                BeginClearSequence(); // ★ 마지막 점화 후 연출 대기 → 다음 레벨/스테이지
            }
        }[SerializeField] float igniteDistance = 0.12f;

        void BeginClearSequence()
        {
            if (_clearPending) return;
            _clearPending = true;

            // 타이머 정지(FAIL 방지)
            if (_levelTimerCo != null) { StopCoroutine(_levelTimerCo); _levelTimerCo = null; }

            // 밝기가 목표치까지 따라붙도록 약간 기다린 뒤 전환
            StartCoroutine(ClearAfterDelay(clearDelayAfterLastLit));
        }

        System.Collections.IEnumerator ClearAfterDelay(float delay)
        {
            float t = delay;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                yield return null;
            }
            _clearPending = false;
            GoToNextInternalLevel();
        }

        void RefreshCounts()
        {
            _needCount = _candles.Count(c => c && c.gameObject.activeInHierarchy);
            _litCount  = _candles.Count(c => c && c.IsLit && c.gameObject.activeInHierarchy);
        }

        void UpdateTargetAlpha()
        {
            float ratio = (_needCount <= 0) ? 1f : Mathf.Clamp01((float)_litCount / _needCount);
            _targetAlpha = Mathf.Lerp(maxDarkAlpha, 0f, ratio);
        }

        void ApplyDimmerImmediate(float a, bool ensureEnabled = false)
        {
            if (_active == null || _active.dimmer == null) return;
            var sr = _active.dimmer;

            if (ensureEnabled)
            {
                sr.enabled = true;
                var col = sr.color; col.r = 0f; col.g = 0f; col.b = 0f; sr.color = col;
                if (useMaskVisibleInside)
                    sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                if (sr.sortingOrder < dimmerSortingOrderMin)
                    sr.sortingOrder = dimmerSortingOrderMin;
            }

            var c = sr.color; c.a = a; sr.color = c;
        }

        void WireAndFixDimmer()
        {
            if (_active == null || _active.root == null) return;

            if (_active.dimmer == null)
            {
                var t = _active.root.transform.Find("CakeDimmer");
                if (t) _active.dimmer = t.GetComponent<SpriteRenderer>();
                if (_active.dimmer == null)
                {
                    _active.dimmer = _active.root
                        .GetComponentsInChildren<SpriteRenderer>(true)
                        .FirstOrDefault(x => x && x.name.ToLower().Contains("dimmer"));
                }
            }

            if (_active.dimmer == null) return;
            ApplyDimmerImmediate(maxDarkAlpha, ensureEnabled: true);
        }

        void OnStageEndedGimmick(bool isCleared)
        {
            Debug.Log(isCleared ? "[CakeFireStage] ► StageEnd: CLEARED" : "[CakeFireStage] ► StageEnd: FAILED");

            var sm = StageManager.Instance;
            if (sm != null && sm.gameObject.activeInHierarchy)
            {
                sm.StageClear(isCleared);
            }
            else
            {
                Debug.LogWarning("[CakeFireStage] StageManager inactive or missing; scene handoff skipped.");
            }
        }
    }
}
