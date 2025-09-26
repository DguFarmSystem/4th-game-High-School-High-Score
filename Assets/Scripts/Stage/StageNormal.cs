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
        protected StageState CurrentStageState = StageState.NotStart;
        private Coroutine _timerCoroutine;
        
        public virtual void OnStageStart()
        {
            CurrentStageState = StageState.Playing;
            _timerCoroutine = StartCoroutine(SetStageTimer(timerTime));
        }
        protected virtual void OnStageEnd()
        {
            // Clear된 스테이지면 true, 아니면 false
            OnStageEnded?.Invoke(CurrentStageState == StageState.Clear);
        }
        
        protected virtual void OnStageClear()
        {
            CurrentStageState = StageState.Clear;
        }

        private IEnumerator SetStageTimer(float time)
        {
            yield return new WaitForSeconds(time);
            if (CurrentStageState != StageState.Clear) CurrentStageState = StageState.Over;
            OnStageEnd();
        }
    }
}