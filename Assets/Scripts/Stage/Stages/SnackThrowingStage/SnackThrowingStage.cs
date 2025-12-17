using UnityEngine;
using UnityEngine.InputSystem;
using Stage;
using System;
using System.Collections;

public class SnackThrowingStage : MonoBehaviour, IStageBase
{
    [SerializeField] private AudioClip _BGMClip;

    public Action<bool> OnStageEnded { get; protected set; }

    protected StageState CurrentStageState = StageState.NotStart;
    public StageState CurrentState => CurrentStageState;

    private int _numberOfStudents = 0;
    
    public void OnStageStart()
    {
        CurrentStageState = StageState.Playing;
    }
    public void OnStageEnd()
    {
        // Clear된 스테이지면 true, 아니면 false
        OnStageEnded?.Invoke(CurrentStageState == StageState.Clear);
    }
    
    public void OnStageClear()
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
        yield return new WaitForSeconds(2f);

        if (isStageCleared)
        {
            Debug.Log("Stage cleared!");

            StageManager.Instance.StageClear(true);
        }
        else
        {
            Debug.Log("Stage failed!");
            StageManager.Instance.StageClear(false);
        }
    }
    
    public void StudentGotSnack()
    {
        _numberOfStudents--;

        if (_numberOfStudents <= 0)
        {
            SetStageClear();
        }
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
        _numberOfStudents = FindObjectsOfType<SnackDetector>().Length;

        // 스테이지 시작
        OnStageStart();
        SoundManager.Instance.PlayBGM(_BGMClip);
    }

    void Update()
    {

    }
}
