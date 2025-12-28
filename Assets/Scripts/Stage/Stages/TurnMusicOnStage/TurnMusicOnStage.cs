using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TurnMusicOnStage : StageNormal
{

    [SerializeField]
    AudioSource TheAudio;
    [SerializeField]
    Scrollbar TheScroll;

    void Start()
    {
        OnStageStart();
    }

    void Update()
    {
        StageGimmik();
        TheAudio.volume = TheScroll.value * 0.5f;
        if (TimerTime<=0&&TheScroll.value>0.77f&&TheScroll.value<0.86)
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
        InputManager.Instance.OnStageTapPerformed += StageGimmik;
        OnStageEnded += OnStageEndedGimmik;
    }
    public void OnDisable()
    {
        InputManager.Instance.OnStageTapPerformed -= StageGimmik;
        OnStageEnded -= OnStageEndedGimmik;
    }
    private void OnStageEndedGimmik(bool isStageCleared)
    {
        InputManager.Instance.OnStageTapPerformed -= StageGimmik;
        TheAudio.Stop();

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
