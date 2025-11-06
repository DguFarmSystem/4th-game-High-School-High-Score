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
    public Image sauceImage;

    [SerializeField]
    private GameObject[] Panels;
    [SerializeField]
    private Sprite[] Backgrounds;
    [SerializeField]
    private Sprite[] FoodSprites;
    [SerializeField]
    private Sprite[] SauceSprites;


    private bool StageClearFlag = false; //스테이지 클리어 여부
    private static int stageLevel = 3; //스테이지 레벨 계산
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
        //stageLevel = StageManager.Instance.GetDifficulty() - 1;
        backgroundImage.sprite = Backgrounds[stageLevel];
        foodImage.sprite = FoodSprites[stageLevel];
        sauceImage.sprite = SauceSprites[stageLevel];
        //Panels[stageLevel].SetActive(true); //칠해야하는 영역 확인용, 나중에 주석 처리
        //active된 panel을 panelDivider의 타겟이 되도록 설정
        panelDivider.targetPanel = Panels[stageLevel].GetComponent<RectTransform>();
        panelDivider.CalculateSubRects();
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
