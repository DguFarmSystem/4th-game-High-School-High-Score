using System;
using System.Collections;
using UnityEngine;

namespace Stage
{
    public class StageNormal : MonoBehaviour, IStageBase
    {
        public Action<bool> OnStageEnded { get; private set; }

        [SerializeField] protected float timerTime = 10.0f;
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
            StopCoroutine(_timerCoroutine);
            CurrentStageState = StageState.Clear;
            OnStageEnd();
        }

        private IEnumerator SetStageTimer(float time)
        {
            yield return new WaitForSeconds(time);
            CurrentStageState = StageState.Over;
            OnStageEnd();
        }
    }
}