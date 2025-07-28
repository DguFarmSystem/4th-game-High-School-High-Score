using UnityEngine;
using UnityEngine.InputSystem;
using Stage;

public class SnackThrowingStage : StageNormal
{
    [SerializeField] public int stageLevel;

    public StageState CurrentState => CurrentStageState;

    // TEST CODE
    [SerializeField] private GameObject _greenSphere;
    [SerializeField] private GameObject _redSphere;

    public override void OnStageStart()
    {
        base.OnStageStart();
    }
    protected override void OnStageEnd()
    {
        base.OnStageEnd();
    }

    protected override void OnStageClear()
    {
        base.OnStageClear();
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
