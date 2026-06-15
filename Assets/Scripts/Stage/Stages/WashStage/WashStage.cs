using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEditor;

public class WashStage : StageNormal
{
    int StageLevel;

    public AudioClip sound;
    AudioSource theaudio;

    public GameObject Shower;
    public Image Background;
    public Sprite[] BackgroundSprite;
    public GameObject[] Hand;
    public int BubbleCount;
    public int CurrentBubbleCount = 0;
    public GameObject Bubble;
    public BoxCollider2D Water;

    bool started = false;

    float TheTime = 4f;
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
        theaudio = GetComponent<AudioSource>();
        Water.enabled = false;
        Shower.transform.position = new Vector3(10.12f, 4.31f, 0);
        StageLevel = StageManager.Instance.GetDifficulty();
        switch (StageLevel)
        {
            case 1:
                Background.sprite = BackgroundSprite[0];
                Hand[0].SetActive(true);
                BubbleCount = 20;
                CreateBubble(BubbleCount, 0);
                break;
            case 2:
                Background.sprite = BackgroundSprite[1];
                Hand[1].SetActive(true);
                BubbleCount = 10;
                CreateBubble(BubbleCount, 1);
                break;
            case 3:
                Background.sprite = BackgroundSprite[0];
                Hand[0].SetActive(true);
                BubbleCount = 30;
                CreateBubble(BubbleCount, 0);
                break;
            case 4:
                Background.sprite = BackgroundSprite[1];
                Hand[1].SetActive(true);
                BubbleCount = 20;
                CreateBubble(BubbleCount, 1);
                break;
        }
        OnStageStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)
        {
            if (InputManager.Instance.TouchedCollider.gameObject == Shower)
            {
                started = true;
                Water.enabled = true;
            }
        }
        TheTime -= Time.deltaTime;
        if (TheTime < 0)
        {
            if (BubbleCount == CurrentBubbleCount)
            {
                OnStageClear();
            }
        }
        else
        {
            if(started)
            Shower.transform.position = InputManager.Instance.TouchWorldPos;
        }
        if (CurrentBubbleCount == BubbleCount) theaudio.volume = 0f;
        else if (CurrentBubbleCount >= BubbleCount * 0.5f) theaudio.volume = 0.05f;
        else if (CurrentBubbleCount >= BubbleCount * 0.3f) theaudio.volume = 0.07f;


    }
    private void OnStageEndedGimmik(bool isStageCleared)
    {
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
    private void CreateBubble(int count, int handtype)
    {
        float xmin, ymin, xmax, ymax,mid1,mid2;
        int CurrentZone=0;
        if (handtype == 0)
        {
            xmin = -6.54f;
            xmax = 6.24f;
            ymin = -2.23f;
            ymax = 2.58f;mid1 = -1.15f;mid2 = 0.94f;
        }
        else
        {
            xmin = -6.86f;
            ymax = 1.87f;
            ymin = -1.92f;
            xmax = 7f;
            mid1 = 3.39f;
            mid2 = 3.91f;
        }
        for(int i = 0; i < count; i++)
        {
            if (CurrentZone == 0)
            {
                GameObject theBubble = Instantiate(Bubble, new Vector3(Random.Range(xmin, mid1), Random.Range(ymin, ymax), 0), Bubble.transform.rotation);
                float scale=Random.Range(0.4f,0.7f);
                theBubble.transform.localScale=new Vector3(scale,scale,0);
                CurrentZone = 1;
            }
            else
            {
                GameObject theBubble = Instantiate(Bubble, new Vector3(Random.Range(mid2, xmax), Random.Range(ymin, ymax), 0), Bubble.transform.rotation);
                float scale = Random.Range(0.4f, 0.7f);
                theBubble.transform.localScale = new Vector3(scale, scale, 0);
                CurrentZone = 0;
            }
        }
    }
}
