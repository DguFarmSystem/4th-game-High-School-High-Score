using System.Collections;
using System.Collections.Generic;
using Stage;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PianoStage : StageNormal
{
    [System.Serializable]
    public class Question
    {
        public int[] index;
    }
    float time;

    int StageLevel;
    [SerializeField]
    Sprite[] BackgroundSprite;
    [SerializeField]
    SpriteRenderer Background;
    int i = -1;

    bool Teaching = true;
    float TeachingTerm = 0.5f;
    [SerializeField]
    Question[] Questions;
    [SerializeField]
    int[] Answer;
    bool IsDifferent = false;
    [SerializeField]
    Sprite[] KeySprite;
    [SerializeField]
    GameObject Effect;
    GameObject TheEffect;
    [SerializeField]
    SpriteRenderer Key;
    [SerializeField]
    Sprite DefaultKey;
    int answercount = 0;
    bool Pressed;
    float Presstime = 0.5f;
    [SerializeField]
    GameObject PlayCommand;

    [SerializeField]
    InputManager InputManager;

    void Start()
    {
        time = 5;
        StageLevel = StageManager.Instance.GetDifficulty();
        Background.sprite = BackgroundSprite[StageLevel - 1];
        Effect = GameObject.Find("Effets");
    }

    void Update()
    {
        if (!Teaching)
        {
            time -= Time.deltaTime;
            if (time < 0)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (Questions[StageLevel - 1].index[j] != Answer[j])
                    {
                        IsDifferent = true;
                    }
                }
                if (!IsDifferent)
                {
                    OnStageClear();
                }
            }
            else
            {
                StageGimmik();
                if (Pressed)
                {
                    Presstime -= Time.deltaTime;
                    if (Presstime <= 0)
                    {
                        Key.sprite = DefaultKey;
                        Pressed = false; Presstime = 0.5f;
                        TheEffect.SetActive(false);//눌렀다가 올라왔을때
                    }
                }
            }
        }
        else
        {
            TeachingTerm -= Time.deltaTime;
            if (TeachingTerm < 0)
            {
                if (i == 3) i++;
                if (i > 3)
                {
                    Teaching = false;
                    OnStageStart();
                    PlayCommand.SetActive(true);
                    Key.sprite = DefaultKey;
                }
                if (i < 3)
                {
                    i++;
                    Key.sprite = KeySprite[Questions[StageLevel - 1].index[i] - 1];
                    TeachingTerm = 0.5f;
                }
            }
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

    private void StageGimmik()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (InputManager.TouchedCollider != null)
        {
            if (InputManager.TouchedCollider.gameObject.tag == "Key")
                OnPianoHit(int.Parse(InputManager.TouchedCollider.gameObject.name));
        }//인풋시스템으로 했을때 - 작동이 안되요,
        //콜라이더가 null로 찍힙니다.
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.tag == "Key")
                {
                    OnPianoHit(int.Parse(hit.transform.gameObject.name));
                }
            }
        }//임시로 레이캐스트로 대체한거,
        //인풋시스템 되면 겟마우스버튼 if문 지우고 레이캐스트 관련 선언한거
        //지우시면 됩니다.
    }

    private void OnPianoHit(int n)
    {
        Key.sprite = KeySprite[n - 1];
        Answer[answercount] = n; answercount++;
        TheEffect = Effect.transform.GetChild(n - 1).gameObject;
        TheEffect.SetActive(true);
        Pressed = true;
    }
}
