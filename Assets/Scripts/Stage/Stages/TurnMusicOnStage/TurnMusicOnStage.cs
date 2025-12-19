using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TurnMusicOnStage : StageNormal
{

    [SerializeField]
    InputManager InputManager;
    [SerializeField]
    AudioSource TheAudio;
    [SerializeField]
    Scrollbar TheScroll;

    float t = 5;

    void Start()
    {
        OnStageStart();
    }

    void Update()
    {
        t -= Time.deltaTime;
        StageGimmik();
        TheAudio.volume = TheScroll.value;
        if (t<=0&&TheScroll.value>0.77f&&TheScroll.value<0.86)
        {
            OnStageClear();
        }
        print(TheScroll.value);
    }

    protected override void OnStageEnd()
    {
        base.OnStageEnd();
    }
    protected override void OnStageClear()
    {
        base.OnStageClear();
    }
    public override void OnStageStart()
    {
        base.OnStageStart();
    }

    public void OnEnable()
    {
        InputManager.OnStageTapPerformed += StageGimmik;
        OnStageEnded += OnStageEndedGimmik;
    }
    public void OnDisable()
    {
        InputManager.OnStageTapPerformed -= StageGimmik;
        OnStageEnded -= OnStageEndedGimmik;
    }
    private void OnStageEndedGimmik(bool isStageCleared)
    {
        InputManager.OnStageTapPerformed -= StageGimmik;
        if (isStageCleared)
        {
            Debug.Log("Cleared");
            StageManager.Instance.StageClear(true);
        }
        else
        {
            Debug.Log("Failed");
            StageManager.Instance.StageClear(false);
        }
    }
    private void StageGimmik()
    {

    }

}
