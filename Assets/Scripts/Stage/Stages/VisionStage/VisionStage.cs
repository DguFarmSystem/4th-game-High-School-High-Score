using System.Collections.Generic;
using UnityEngine;

namespace Stage
{
    [DisallowMultipleComponent]
    public class VisionStage : StageNormal
    {
        [System.Serializable]
        public class LevelEntry
        {
            public GameObject root;

            [Header("Answer Selector")]
            public VisionAnswerSelector answerSelector;

            [Header("Correct Answer")]
            [Range(0, 3)]
            public int correctAnswerIndex;
        }

        [Header("Levels")]
        [SerializeField] private List<LevelEntry> levels = new List<LevelEntry>();

        [Header("DEBUG (Editor/Dev only)")]
        [SerializeField] private bool debugForceDifficulty = false;

        [Range(1, 4)]
        [SerializeField] private int debugDifficulty = 1;

        [Header("Sound")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioClip selectSfxClip;

        private int _activeIndex = -1;
        private LevelEntry _active;

        private bool _reportedToStageManager;

        private void OnEnable()
        {
            OnStageEnded += OnStageEndedGimmick;
        }

        private void OnDisable()
        {
            OnStageEnded -= OnStageEndedGimmick;

            foreach (var level in levels)
            {
                if (level != null && level.answerSelector != null)
                    level.answerSelector.OnSelected -= HandleSelect;
            }
        }

        private void Start()
        {
            if (CurrentStageState == StageState.NotStart)
                OnStageStart();
        }

        public override void OnStageStart()
        {
            _reportedToStageManager = false;

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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (debugForceDifficulty)
            {
                SetupLevel(_activeIndex);
            }
            else
#endif
            {
                if (StageManager.Instance != null)
                    SetupLevel(StageManager.Instance.GetDifficulty() - 1);
                else
                    SetupLevel(_activeIndex);
            }

            if (bgmSource != null && !bgmSource.isPlaying)
                bgmSource.Play();

            base.OnStageStart();
        }

        private void SetupLevel(int index)
        {
            index = Mathf.Clamp(index, 0, Mathf.Max(0, levels.Count - 1));

            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i].root != null)
                    levels[i].root.SetActive(i == index);

                if (levels[i].answerSelector != null)
                    levels[i].answerSelector.OnSelected -= HandleSelect;
            }

            _activeIndex = index;
            _active = (index >= 0 && index < levels.Count) ? levels[index] : null;

            if (_active != null && _active.answerSelector != null)
            {
                _active.answerSelector.OnSelected += HandleSelect;
                _active.answerSelector.Initialize();
                _active.answerSelector.SetTouchEnabled(true);
            }

            Debug.Log($"[VisionStage] SetupLevel 완료 / Level = {index + 1}");
        }

        private void HandleSelect()
        {
            if (sfxSource == null || selectSfxClip == null) return;

            sfxSource.Stop();
            sfxSource.clip = selectSfxClip;
            sfxSource.Play();
        }

        protected override void OnStageEnd()
        {
            bool isCorrect = CheckAnswer();

            if (_active != null && _active.answerSelector != null)
                _active.answerSelector.SetTouchEnabled(false);

            if (bgmSource != null && bgmSource.isPlaying)
                bgmSource.Stop();

            if (isCorrect)
            {
                CurrentStageState = StageState.Clear;
                Debug.Log("[VisionStage] 정답 판정: CLEAR");
            }
            else
            {
                CurrentStageState = StageState.Over;
                Debug.Log("[VisionStage] 정답 판정: FAIL");
            }

            base.OnStageEnd();
        }

        private bool CheckAnswer()
        {
            if (_active == null || _active.answerSelector == null)
                return false;

            int selectedIndex = _active.answerSelector.GetCurrentIndex();

            Debug.Log($"[VisionStage] selected = {selectedIndex}, correct = {_active.correctAnswerIndex}");

            return selectedIndex == _active.correctAnswerIndex;
        }

        private void OnStageEndedGimmick(bool isCleared)
        {
            if (_reportedToStageManager) return;
            _reportedToStageManager = true;

            Debug.Log(isCleared
                ? "[VisionStage] ► StageEnd: CLEARED"
                : "[VisionStage] ► StageEnd: FAILED");

            StageManager sm = StageManager.Instance;

            if (sm != null && sm.gameObject.activeInHierarchy)
            {
                sm.StageClear(isCleared);
            }
            else
            {
                Debug.LogWarning("[VisionStage] StageManager inactive or missing; scene handoff skipped.");
            }
        }
    }
}