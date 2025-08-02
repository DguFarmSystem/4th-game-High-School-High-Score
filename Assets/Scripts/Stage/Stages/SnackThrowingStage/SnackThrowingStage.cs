using UnityEngine;
using UnityEngine.InputSystem;
using Stage;
using System;

public class SnackThrowingStage : MonoBehaviour, IStageBase
{
    public Action<bool> OnStageEnded { get; protected set; }

    protected StageState CurrentStageState = StageState.NotStart;
    public StageState CurrentState => CurrentStageState;

    private int _numberOfStudents = 0;

    // TEST CODE
    [SerializeField] private GameObject _greenSphere;
    [SerializeField] private GameObject _redSphere;
    
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

        if (isStageCleared)
        {
            //TEST CODE
            Debug.Log("Stage cleared!");
            _greenSphere.SetActive(true);
        }
        else
        {
            //TEST CODE
            Debug.Log("Stage failed!");
            _redSphere.SetActive(true);
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
    }

    void Update()
    {
        /*
        if (/*스테이지 클리어 조건 만족 &&
        CurrentStageState == StageState.Playing)
        {
            OnStageClear(); // 모든 조건이 완료되면 스테이지 클리어 처리
        }
        */
    }
}
