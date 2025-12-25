using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Stage;

public class MusicGuitarStage : StageNormal
{
    [SerializeField]
    private PanelTouchHandler PickHandler;
    [SerializeField]
    private Image CrowdImage;
    [SerializeField]
    private List<Sprite> CrowdSprite;
    [SerializeField]
    private List<int> Goal;

    private int CurrentCount = 0;
    private bool StageClearFlag = false;
    private static int stageLevel = 0;

    public override void OnStageStart()
    {
        base.OnStageStart();
    }
    protected override void OnStageEnd()
    {
        //Debug.Log("Stage Failed.. Current Level : " + stageLevel);
        base.OnStageEnd();
    }

    protected override void OnStageClear()
    {
        //stageLevel++;
        //Debug.Log("Stage Cleared!! Current Level : " + stageLevel);
        base.OnStageClear();
    }

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
        stageLevel = StageManager.Instance.GetDifficulty() - 1;
        CrowdImage.sprite = CrowdSprite[0];
        OnStageStart();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentCount = PickHandler.GetPlayCount();
        ChangeCrowdImage();
        if(CurrentCount >= Goal[stageLevel])
        {
            StageClearFlag = true;
        }

        if(StageClearFlag && CurrentStageState == StageState.Playing)
        {
            OnStageClear();
        }
    }

    void ChangeCrowdImage()
    {
        float ClearRatio = (float)CurrentCount / Goal[stageLevel];

        Debug.Log($"[ChangeCrowdImage] CurrentCount: {CurrentCount}, Goal: {Goal[stageLevel]}, ClearRatio: {ClearRatio:F2}");

        if (ClearRatio <= 0.25f)
        {
            //Debug.Log("[ChangeCrowdImage] Range: <= 0.25 (Sprite 0)");
            CrowdImage.sprite = CrowdSprite[0];
        }
        else if (ClearRatio > 0.25f && ClearRatio <= 0.5f)
        {
            //Debug.Log("[ChangeCrowdImage] Range: 0.25 ~ 0.5 (Sprite 1)");
            CrowdImage.sprite = CrowdSprite[1];
        }
        else if (ClearRatio > 0.5f && ClearRatio <= 0.75f)
        {
            //Debug.Log("[ChangeCrowdImage] Range: 0.5 ~ 0.75 (Sprite 2)");
            CrowdImage.sprite = CrowdSprite[2];
        }
        else
        {
            //Debug.Log("[ChangeCrowdImage] Range: > 0.75 (Sprite 3)");
            CrowdImage.sprite = CrowdSprite[3];
        }
    }
}
