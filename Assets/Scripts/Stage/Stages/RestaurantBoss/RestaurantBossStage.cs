using System;
using System.Collections;
using System.Collections.Generic;
using Stage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestaurantBossStage : MonoBehaviour, IStageBase
{
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    [SerializeField] private GameObject _timerBarNGoal;

    [SerializeField] public int ClearItemCount { get; private set; }= 150;

    [SerializeField] private AudioClip _stageBGM;

    public float StageTimeLimit { get; private set; } = 30f; // 30초

    public Action<bool> OnStageEnded { get; protected set; }

    protected StageState CurrentStageState = StageState.NotStart;
    public StageState CurrentState => CurrentStageState;
    
    public void OnStageStart()
    {
        CurrentStageState = StageState.Playing;
    }
    private void OnStageEnd()
    {
        // Clear된 스테이지면 true, 아니면 false
        OnStageEnded?.Invoke(CurrentStageState == StageState.Clear);
    }
    
    private void OnStageClear()
    {
        CurrentStageState = StageState.Clear;
        OnStageEnd();
    }

    public void SetStageClear()
    {
        OnStageClear();
    }

    public void SetStageFailed()
    {
        CurrentStageState = StageState.Over;
        OnStageEnd();
    }
    
    private void OnStageEndedGimmik(bool isStageCleared)
    {
        StartCoroutine(StageEndedGimmik(isStageCleared));
    }

    private IEnumerator StageEndedGimmik(bool isStageCleared)
    {
        if (isStageCleared)
        {
            //TEST CODE
            Debug.Log("Stage cleared!");

            yield return new WaitUntil(() => StageTimeLimit <= 0f);

            StageManager.Instance.StageClear(true);
        }
        else
        {
            //TEST CODE
            Debug.Log("Stage failed!");

            yield return new WaitForSeconds(2f);

            StageManager.Instance.StageClear(false);
        }
    }

    private IEnumerator DelayedStart()
    {
        SoundManager.Instance.PlayBGM(_stageBGM);
        yield return new WaitForSeconds(2.3f); // 2.3초 대기
        
        _leftButton.interactable = true;
        _rightButton.interactable = true;

        TextMeshProUGUI[] _timerTexts = _timerBarNGoal.GetComponentsInChildren<TextMeshProUGUI>(true);
        _timerTexts[1].text = ClearItemCount.ToString();
        _timerBarNGoal.SetActive(true);

        // 스테이지 시작
        OnStageStart();
    }

    public void GetExtraTime(float extraTime)
    {
        StageTimeLimit += extraTime;
    }

    // ============ Lifecycle methods ============ //

    public void OnEnable()
    {
        OnStageEnded += OnStageEndedGimmik;
    }

    public void OnDisable()
    {
        OnStageEnded -= OnStageEndedGimmik;
    }

    void Start()
    {
        StartCoroutine(DelayedStart());
    }

    void Update()
    {
        if (CurrentStageState != StageState.Playing && CurrentStageState != StageState.Clear) return;
        
        StageTimeLimit -= Time.deltaTime;

        if (CurrentStageState == StageState.Playing && LeftRightBtn.CorrectInputCount >= ClearItemCount)
        {
            SetStageClear();
        }

        if (CurrentStageState == StageState.Playing && StageTimeLimit <= 0f)
        {
            SetStageFailed();
        }
    }
}
