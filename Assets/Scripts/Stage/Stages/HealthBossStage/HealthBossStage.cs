using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage;

public class HealthBossStage : StageNormal
{
    [Header("Command")]
    public CommandProcessor Command;

    [Header("MiniGame")]
    public HeartLineDrawing LineStage;
    public CableEvent ShockingStage;
    public PumpAction PumpStage;

    [Header("UI Components")]
    public GameObject HoseUI;
    public GameObject CableUI;
    public GameObject PumpUI;

    private bool isLineClear = false;
    private bool isShockingClear = false;
    private bool isPumpingClear = false;

    private bool CableCommandCalled = false;
    private bool PumpCommandCalled = false;

    private void OnStageEndedGimmik(bool isStageCleared)
    {

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

    // ============ Lifecycle methods ============ //
    public void OnEnable()
    {
        OnStageEnded += OnStageEndedGimmik;
    }

    public void OnDisable()
    {
        OnStageEnded -= OnStageEndedGimmik;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckGameClear();
        if(isLineClear && isShockingClear && isPumpingClear)
        {
            StartCoroutine(MakeEndDelay(2.0f));
        }
        else if (isLineClear && isShockingClear)
        {
            StartCoroutine(MakePumpDelay(0.7f));
        }
        else if (isLineClear)
        {
            StartCoroutine(MakeCableDelay(0.7f));
        }
    }

    void CheckGameClear()
    {
        isLineClear = LineStage.GetLineClear();
        isShockingClear = ShockingStage.GetCableClear();
        isPumpingClear = PumpStage.GetPumpClear();
    }

    void SetCableGame()
    {
        CableUI.SetActive(true);
        if(!CableCommandCalled)
        {
            Command.ChangeCommand(1);
            CableCommandCalled = true;
        }
    }

    void SetPumpGame()
    {
        PumpUI.SetActive(true);
        if(!PumpCommandCalled)
        {
            Command.ChangeCommand(2);
            PumpCommandCalled = true;
        }
    }

    IEnumerator MakeCableDelay(float DelayTime)
    {
        yield return new WaitForSeconds(DelayTime);
        SetCableGame();
    }

    IEnumerator MakePumpDelay(float DelayTime)
    {
        yield return new WaitForSeconds(DelayTime);
        SetPumpGame();
    }

    IEnumerator MakeEndDelay(float DelayTime)
    {
        yield return new WaitForSeconds(DelayTime);
        OnStageEnded.Invoke(true);
    }
}
