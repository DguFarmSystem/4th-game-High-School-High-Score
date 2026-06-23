using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class ScalesStage : StageNormal
{
    int StageLevel;
    [SerializeField]
    Sprite[] BackgroundSprite;
    [SerializeField]
    Sprite[] ProblemScales;
    [SerializeField]
    SpriteRenderer ProblemScaleOBJ;
    [SerializeField]
    int[] ProblemNumList;
    int ProblemNum;
    [SerializeField]
    Image Background;
    AudioSource theaudio;
    [SerializeField]
    GameObject[] Weigh;
    InputAction.CallbackContext context;
    GameObject TheWeigh;

    [SerializeField]
    Text[] text;

    bool Spawned = false;
    int answer;

    float timer = 5f;

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
        StageLevel = StageManager.Instance.GetDifficulty();
        Background.sprite = BackgroundSprite[StageLevel - 1];
        ProblemScaleOBJ.sprite=ProblemScales[StageLevel - 1];
        ProblemNum = ProblemNumList[StageLevel - 1];
        theaudio = GetComponent<AudioSource>();
        if(ProblemNum<10)
            text[0].text= "0"+ProblemNum.ToString();
        else
            text[0].text = ProblemNum.ToString();
        OnStageStart();
    }

    // Update is called once per frame
    void Update()
    {
        if(answer<10)
        text[1].text = "0"+answer.ToString();
        else text[1].text = answer.ToString();
            timer -= Time.deltaTime;
        if(timer < 0 )
        {
            if(ProblemNum==answer)OnStageClear();
        }
        if (InputManager.Instance.IsPressing)
        {
            if (!Spawned && InputManager.Instance.TouchedCollider != null)
            {
                switch (InputManager.Instance.TouchedCollider.name)
                {
                    case "2Spawner":
                        TheWeigh = Instantiate(Weigh[0], InputManager.Instance.TouchWorldPos, new Quaternion(0, 0, 0, 0));
                        break;
                    case "5Spawner":
                        TheWeigh = Instantiate(Weigh[1], InputManager.Instance.TouchWorldPos, new Quaternion(0, 0, 0, 0));
                        break;
                    case "10Spawner":
                        TheWeigh = Instantiate(Weigh[2], InputManager.Instance.TouchWorldPos, new Quaternion(0, 0, 0, 0));
                        break;
                    default:
                        TheWeigh = InputManager.Instance.TouchedCollider.gameObject;
                        break;
                }
                Spawned = true;
                TheWeigh.GetComponent<BoxCollider2D>().enabled = false;
            }
            else
            {
                if (TheWeigh != null)
                {
                    TheWeigh.transform.position = InputManager.Instance.TouchWorldPos;
                    TheWeigh.GetComponent<Weight>().set = false;
                }
            }
        }
        else
        {
            Spawned = false;
            if (TheWeigh != null)
            {
                TheWeigh.GetComponent<BoxCollider2D>().enabled = true;
                TheWeigh = null;
            }
        }
    }
    private void OnStageEndedGimmik(bool isStageCleared)
    {
        theaudio.Stop();

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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.name)
        {
            case "2(Clone)":
                answer += 2;
                break;
            case "5(Clone)":
                answer += 5;
                break;
            case "10(Clone)":
                answer += 10;
                break;
        }
        collision.gameObject.GetComponent<Weight>().IsInRange = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.name)
        {
            case "2(Clone)":
                answer -= 2;
                break;
            case "5(Clone)":
                answer -= 5;
                break;
            case "10(Clone)":
                answer -= 10;
                break;
        }
        collision.gameObject.GetComponent<Weight>().IsInRange = false;
    }
}
