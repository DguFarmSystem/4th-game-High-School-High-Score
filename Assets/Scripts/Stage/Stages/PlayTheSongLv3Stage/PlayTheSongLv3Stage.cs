using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stage
{
    [DisallowMultipleComponent]
    public class PlayTheSongLv3Stage : StageNormal
    {
        [Header("Rule")]
        [SerializeField] int requiredPressCount = 4;

        [Header("모니터")]
        [Tooltip("깨진 화면")]
        [SerializeField] GameObject monitorGlitchObject;

        [Header("숫자 4 아이콘 슬롯들")]
        [SerializeField] List<GameObject> numberSlots = new List<GameObject>();

        [Header("Sound")]
        [SerializeField] AudioSource bgmSource;
        [SerializeField] AudioSource sfxSource;
        [SerializeField] AudioClip fourButtonSfxClip;

        int  _currentCount = 0;
        bool _screenBroken = false;
        bool _isCleared    = false;

        void OnEnable()  => OnStageEnded += OnStageEndedGimmick;
        void OnDisable() => OnStageEnded -= OnStageEndedGimmick;

        void Start()
        {
            if (CurrentStageState == StageState.NotStart)
                OnStageStart();
        }

        public override void OnStageStart()
        {
            base.OnStageStart();

            _currentCount = 0;
            _screenBroken = false;
            _isCleared    = false;

            // 노이즈 화면 끄기
            if (monitorGlitchObject)
                monitorGlitchObject.SetActive(false);

            // 4 아이콘 전부 끄기
            foreach (var go in numberSlots)
                if (go) go.SetActive(false);

            if (bgmSource && !bgmSource.isPlaying)
                bgmSource.Play();
        }

        public void OnNumberButtonPressed(int number)
        {
            if (CurrentStageState != StageState.Playing) return;
            if (number != 4) return;   // 4 아니면 무시

            if (fourButtonSfxClip)
            {
                if (sfxSource) sfxSource.PlayOneShot(fourButtonSfxClip);
                else if (bgmSource) bgmSource.PlayOneShot(fourButtonSfxClip);
            }

            // 첫 4 입력 시 노이즈 화면 켜기
            if (!_screenBroken)
            {
                _screenBroken = true;
                if (monitorGlitchObject)
                    monitorGlitchObject.SetActive(true);

                Debug.Log("[PlayTheSongLv3Stage] 첫 4 입력");
            }

            // 4 아이콘 하나씩 켜기
            if (_currentCount < numberSlots.Count)
            {
                var slot = numberSlots[_currentCount];
                if (slot) slot.SetActive(true);
            }

            _currentCount++;

            Debug.Log($"[PlayTheSongLv3Stage] 4 입력 횟수: {_currentCount}/{requiredPressCount}");

            if (_currentCount >= requiredPressCount) {
                _isCleared = true;

                // 4444를 모두 입력 시 클리어 조건 달성
                Debug.Log("[PlayTheSongLv3Stage] CLEAR 조건 달성");
            }
        }

        protected override void OnStageEnd()
        {
            if (bgmSource && bgmSource.isPlaying)
                bgmSource.Stop();

            if (_isCleared)
            {
                CurrentStageState = StageState.Clear;
            }
            else if (CurrentStageState == StageState.Playing)
            {
                CurrentStageState = StageState.Over;
            }

            // 클리어 && 타이머 종료 시 OnStageEnd() 호출
            Debug.Log($"[PlayTheSongLv3Stage] OnStageEnd() 호출, isCleared={_isCleared}, state={CurrentStageState}");

            base.OnStageEnd();
        }

        void OnStageEndedGimmick(bool isCleared)
        {
            // 클리어 여부 로그
            Debug.Log(isCleared
                ? "[PlayTheSongLv3Stage] ► StageEnd: CLEARED"
                : "[PlayTheSongLv3Stage] ► StageEnd: FAILED");

            var sm = StageManager.Instance;
            if (sm != null && sm.gameObject.activeInHierarchy)
            {
                sm.StageClear(isCleared);
            }
            else
            {
                Debug.LogWarning("[PlayTheSongLv3Stage] StageManager inactive or missing");
            }
        }
    }
}
