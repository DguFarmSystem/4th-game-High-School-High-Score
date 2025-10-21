using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Stage;

public class RestaurantSpreadStage : StageNormal
{
    public PanelDivider panelDivider;
    public Image backgroundImage;
    public Image foodImage;

    [SerializeField]
    private GameObject[] Panels;
    [SerializeField]
    private Sprite[] Backgrounds;
    [SerializeField]
    private Sprite[] FoodSprites;


    private bool StageClearFlag = false; //스테이지 클리어 여부
    private static int stageLevel = 1; //스테이지 레벨 계산
    public override void OnStageStart()
    {
        base.OnStageStart();
    }
    protected override void OnStageEnd()
    {
        Debug.Log("Stage Failed.. Current Level : " + stageLevel);
        base.OnStageEnd();
    }

    protected override void OnStageClear()
    {
        stageLevel++;
        Debug.Log("Stage Cleared!! Current Level : " + stageLevel);
        base.OnStageClear();
    }

    // Start is called before the first frame update
    void Start()
    {
        backgroundImage.sprite = Backgrounds[stageLevel];
        foodImage.sprite = FoodSprites[stageLevel];
        Panels[stageLevel].SetActive(true);
        //active된 panel을 panelDivider의 타겟이 되도록 설정
        panelDivider.targetPanel = Panels[stageLevel].GetComponent<RectTransform>();
        Debug.Log("Stage Started!! Level : " + stageLevel);
        OnStageStart();
    }

    // Update is called once per frame
    void Update()
    {
        if(panelDivider.AllVisited())
        {
            StageClearFlag = true;
        }

        if(StageClearFlag && CurrentStageState == StageState.Playing)
        {
            OnStageClear();
        }

        //if(timerTime <= 0.0f)
        //{
        //    OnStageEnd();
        //}
    }
}
