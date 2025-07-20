using System;
using System.Collections;
using UnityEngine;

namespace Stage
{
    public class StageNormal : MonoBehaviour, IStageBase
    {
        public Action<bool> OnStageEnded { get; protected set; }

        [SerializeField] protected float timerTime;// 스테이지 타이머 시간 (초 단위)
        public float TimerTime => timerTime;
        protected StageState currentStageState = StageState.NotStart;
        private Coroutine _timerCoroutine;
        
        public virtual void OnStageStart()
        {
            currentStageState = StageState.Playing;
            _timerCoroutine = StartCoroutine(SetStageTimer(timerTime));
        }
        protected virtual void OnStageEnd()
        {
            // Clear된 스테이지면 true, 아니면 false
            OnStageEnded?.Invoke(currentStageState == StageState.Clear);
        }
        
        protected virtual void OnStageClear()
        {
            StopCoroutine(_timerCoroutine);
            currentStageState = StageState.Clear;
            OnStageEnd();
        }

        private IEnumerator SetStageTimer(float time)
        {
            yield return new WaitForSeconds(time);
            currentStageState = StageState.Over;
            OnStageEnd();
        }
    }
}