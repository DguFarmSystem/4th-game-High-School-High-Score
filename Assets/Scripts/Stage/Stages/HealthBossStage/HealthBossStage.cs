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
    public VitalProcessor Vital;

    [Header("Audio Resource")]
    public AudioSource VitalSource;
    public AudioClip DeadSound;

    private bool isLineClear = false;
    private bool isShockingClear = false;
    private bool isPumpingClear = false;

    private bool CableCommandCalled = false;
    private bool PumpCommandCalled = false;

    private float StageTimer = 0.0f;
    private bool GameOver = false;
    private Coroutine GameOverCoroutine;

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
        CheckGameOver();
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
        StageTimer += Time.deltaTime;
    }

    void CheckGameClear()
    {
        isLineClear = LineStage.GetLineClear();
        isShockingClear = ShockingStage.GetCableClear();
        isPumpingClear = PumpStage.GetPumpClear();
    }

    void CheckGameOver()
    {
        //1. 10초 이내에 isLineClear가 true가 되는지 check
        if (!isLineClear && StageTimer > 12.0f)
        {
            GameOver = true;
        }
        //2. 5초 이내에 isShockingClear가 true가 되는지 check
        else if (!isShockingClear && StageTimer > 19.0f)
        {
            GameOver = true;
        }
        else if (!isPumpingClear && StageTimer > 30.0f)
        {
            GameOver = true;
        }

        if (GameOver)
        {
            Command.TouchBlocker.SetActive(true);
            Vital.ChangeImage(3);
            if(GameOverCoroutine == null)
            {
                GameOverCoroutine = StartCoroutine(GameOverDelay());
            }
        }
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

    IEnumerator GameOverDelay()
    {
        VitalSource.clip = DeadSound;
        VitalSource.loop = false;
        VitalSource.Play();
        yield return new WaitForSeconds(3f);
        VitalSource.Stop();
        OnStageEnded.Invoke(false);
    }
}
