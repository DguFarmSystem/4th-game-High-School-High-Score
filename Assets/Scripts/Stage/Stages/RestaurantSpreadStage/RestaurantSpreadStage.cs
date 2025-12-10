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


    private bool StageClearFlag = false; //�������� Ŭ���� ����
    private static int stageLevel = 0; //�������� ���� ���
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
        backgroundImage.sprite = Backgrounds[stageLevel];
        foodImage.sprite = FoodSprites[stageLevel];
        AdjustSauceRatio(stageLevel);
        sauceImage.sprite = SauceSprites[stageLevel];
        //Panels[stageLevel].SetActive(true); //ĥ�ؾ��ϴ� ���� Ȯ�ο�, ���߿� �ּ� ó��
        //active�� panel�� panelDivider�� Ÿ���� �ǵ��� ����
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

    void AdjustSauceRatio(int stagelevel)
    {
        RectTransform rt = sauceImage.GetComponent<RectTransform>();
        switch(stagelevel)
        {
            case 0: case 1:
                {
                    rt.sizeDelta = new Vector2(1424, 2084);
                    rt.localScale = new Vector3(0.2f, 0.2f, 1.0f);
                    break;
                }

            //case 1:
            //    {
            //        rt.sizeDelta = new Vector2(1424, 2084);
            //        rt.localScale = new Vector3(0.4f, 0.4f, 1.0f);
            //        break;
            //    }

            case 2:
                {
                    rt.sizeDelta = new Vector2(1000, 914);
                    rt.localScale = new Vector3(0.4f, 0.4f, 1.0f);
                    break;
                }

            case 3:
                {
                    rt.sizeDelta = new Vector2(1100, 900);
                    rt.localScale = new Vector3(0.4f, 0.4f, 1.0f);
                    break;
                }
        }
    }
}
