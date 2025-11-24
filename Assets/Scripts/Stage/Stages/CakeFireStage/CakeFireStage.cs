// Assets/Scripts/Stage/Stages/CakeFireStage/CakeFireStage.cs
using System.Collections;
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
            public GameObject root;          // Cake_Lv1, Lv2…
            public SpriteRenderer dimmer;    // CakeDimmer (SpriteRenderer)
        }

        [Header("Levels (난이도별 케이크)")]
        [SerializeField] List<LevelEntry> levels = new List<LevelEntry>();

        [Header("Dimmer Settings")]
        [Range(0f, 1f)] [SerializeField] float maxDarkAlpha = 0.95f;
        [SerializeField] float dimmerLerpSpeed = 6f;

        [Header("Render Safety")]
        [SerializeField] int  dimmerSortingOrderMin = 2000;
        [SerializeField] bool useMaskVisibleInside = true;

        [Header("Clear Delay")]
        [Tooltip("마지막 점화 후 연출 대기")]
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

        bool _clearPending;
        bool _reportedToStageManager = false; // 중복 보고 방지

        void OnEnable()  => OnStageEnded += OnStageEndedGimmick;
        void OnDisable() => OnStageEnded -= OnStageEndedGimmick;

        void Start()
        {
            if (CurrentStageState == StageState.NotStart)
                OnStageStart();
        }

        public override void OnStageStart()
        {
            _reportedToStageManager = false;

            int diff = 1;
            if (StageManager.Instance != null)
                diff = Mathf.Max(1, StageManager.Instance.GetDifficulty());

            _activeIndex = Mathf.Clamp(diff - 1, 0, Mathf.Max(0, levels.Count - 1));

            StopAllCoroutines();
            _clearPending = false;
            CurrentStageState = StageState.Playing;

            SetupLevel(_activeIndex);

            base.OnStageStart();
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
                    _currentAlpha, _targetAlpha, ref _alphaVel,
                    dimmerSmoothTime,
                    dimmerMaxSpeed <= 0f ? Mathf.Infinity : dimmerMaxSpeed,
                    Time.deltaTime
                );
                ApplyDimmerImmediate(_currentAlpha);
            }
        }

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

        // 내부 레벨 자동 전환 로직 (일단 남겨만 뒀습니다)
        void GoToNextInternalLevel()
        {
            if (_activeIndex + 1 < levels.Count)
            {
                Debug.Log($"[CakeFireStage] ► Next Level would be Lv{_activeIndex + 2} (auto-advance disabled)");
                return;
            }
            Debug.Log("[CakeFireStage] ► Stage Cleared (all internal levels)!");
        }

        IEnumerator EndAfter(float t)
        {
            yield return new WaitForSeconds(t);
            OnStageEnd();
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
                BeginClearSequence(); // 마지막 점화 후 약간의 연출 대기
            }
        }

        [SerializeField] float igniteDistance = 0.12f;

        void BeginClearSequence()
        {
            if (_clearPending) return;
            _clearPending = true;

            StartCoroutine(ClearAfterDelay(clearDelayAfterLastLit));
        }

        IEnumerator ClearAfterDelay(float delay)
        {
            float t = delay;
            while (t > 0f) { t -= Time.deltaTime; yield return null; }

            _clearPending = false;

            // 상태만 Clear로 바꿔둠
            CurrentStageState = StageState.Clear;
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
            if (_reportedToStageManager) return;
            _reportedToStageManager = true;

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