using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stage
{
    [DisallowMultipleComponent]
    public class AttachStage : StageNormal
    {
        [System.Serializable]
        public class LevelEntry
        {
            public GameObject root;
        }

        [Header("Levels")]
        [SerializeField] private List<LevelEntry> levels = new List<LevelEntry>();

        [Header("Clear Settings")]
        [SerializeField] private float clearDelay = 0.3f;

        [Header("DEBUG (Editor/Dev only)")]
        [SerializeField] private bool debugForceDifficulty = false;

        [Range(1, 4)]
        [SerializeField] private int debugDifficulty = 1;

        private int _activeIndex = -1;
        private LevelEntry _active;

        private readonly List<AttachableObject> _targets = new();

        private int _needCount;
        private int _attachedCount;

        private bool _clearPending;
        private bool _reportedToStageManager;

        private void OnEnable()
        {
            OnStageEnded += OnStageEndedGimmick;
        }

        private void OnDisable()
        {
            OnStageEnded -= OnStageEndedGimmick;
        }

        private void Start()
        {
            if (CurrentStageState == StageState.NotStart)
                OnStageStart();
        }

        public override void OnStageStart()
        {
            _reportedToStageManager = false;
            _clearPending = false;
            _attachedCount = 0;
            _needCount = 0;

            int diff = 1;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (debugForceDifficulty)
            {
                diff = Mathf.Max(1, debugDifficulty);
            }
            else
#endif
            {
                if (StageManager.Instance != null)
                    diff = Mathf.Max(1, StageManager.Instance.GetDifficulty());
            }

            _activeIndex = Mathf.Clamp(diff - 1, 0, Mathf.Max(0, levels.Count - 1));

            StopAllCoroutines();
            CurrentStageState = StageState.Playing;

            SetupLevel(_activeIndex);

            base.OnStageStart();
        }

        protected override void OnStageEnd()
        {
            base.OnStageEnd();
        }

        private void SetupLevel(int index)
        {
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i].root != null)
                    levels[i].root.SetActive(i == index);
            }

            _active = (index >= 0 && index < levels.Count) ? levels[index] : null;

            _targets.Clear();

            if (_active != null && _active.root != null)
            {
                AttachableObject[] found = _active.root.GetComponentsInChildren<AttachableObject>(true);
                _targets.AddRange(found);

                MagnetController magnet = _active.root.GetComponentInChildren<MagnetController>(true);
                if (magnet != null)
                    magnet.Initialize(this);
                else
                    Debug.LogWarning("[AttachStage] MagnetController를 찾지 못했어.");
            }

            RefreshCounts();

            Debug.Log($"[AttachStage] SetupLevel 완료 / needCount = {_needCount}");
        }

        private void RefreshCounts()
        {
            _needCount = 0;
            _attachedCount = 0;

            foreach (AttachableObject target in _targets)
            {
                if (target == null || !target.gameObject.activeInHierarchy)
                    continue;

                _needCount++;

                if (target.IsAttached)
                    _attachedCount++;
            }
        }

        public void NotifyObjectAttached(AttachableObject obj)
        {
            if (obj == null) return;
            if (_clearPending) return;

            RefreshCounts();

            Debug.Log($"[AttachStage] 부착 진행 {_attachedCount}/{_needCount}");

            if (_needCount > 0 && _attachedCount >= _needCount)
            {
                BeginClearSequence();
            }
        }

        private void BeginClearSequence()
        {
            if (_clearPending) return;

            _clearPending = true;
            StartCoroutine(ClearAfterDelay(clearDelay));
        }

        private IEnumerator ClearAfterDelay(float delay)
        {
            float t = delay;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                yield return null;
            }

            _clearPending = false;
            CurrentStageState = StageState.Clear;
        }

        private void OnStageEndedGimmick(bool isCleared)
        {
            if (_reportedToStageManager) return;
            _reportedToStageManager = true;

            Debug.Log(isCleared
                ? "[AttachStage] ► StageEnd: CLEARED"
                : "[AttachStage] ► StageEnd: FAILED");

            StageManager sm = StageManager.Instance;
            if (sm != null && sm.gameObject.activeInHierarchy)
            {
                sm.StageClear(isCleared);
            }
            else
            {
                Debug.LogWarning("[AttachStage] StageManager inactive or missing; scene handoff skipped.");
            }
        }
    }
}