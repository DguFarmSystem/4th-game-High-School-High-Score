using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;

public class MusicDanceStage : StageNormal
{
    [Header("GameObject Settings")]
    [SerializeField] private GameObject Objective;
    [SerializeField] private GameObject Button1;
    [SerializeField] private GameObject Button2;
    [SerializeField] private GameObject Button3;
    [SerializeField] private GameObject Button4;

    [Space(10)]
    
    [Header("LV1 Settings")]
    [SerializeField] private Sprite ObjectiveSprite1;
    [SerializeField] private Sprite Answer1_1;
    [SerializeField] private Sprite Answer1_2;
    [SerializeField] private Sprite Answer1_3;
    [SerializeField] private Sprite Answer1_4;

    [Space(10)]

    [Header("LV2 Settings")]
    [SerializeField] private Sprite ObjectiveSprite2;
    [SerializeField] private Sprite Answer2_1;
    [SerializeField] private Sprite Answer2_2;
    [SerializeField] private Sprite Answer2_3;
    [SerializeField] private Sprite Answer2_4;

    [Space(10)]
    [Header("LV3 Settings")]
    [SerializeField] private Sprite ObjectiveSprite3;
    [SerializeField] private Sprite Answer3_1;
    [SerializeField] private Sprite Answer3_2;
    [SerializeField] private Sprite Answer3_3;
    [SerializeField] private Sprite Answer3_4;

    [Space(10)]
    [Header("LV4 Settings")]
    [SerializeField] private Sprite ObjectiveSprite4;
    [SerializeField] private Sprite Answer4_1;
    [SerializeField] private Sprite Answer4_2;
    [SerializeField] private Sprite Answer4_3;
    [SerializeField] private Sprite Answer4_4;

    [Space(10)]
    [Header("BGM Settings")]
    [SerializeField] private AudioClip _stageBGM;

    private List<GameObject> buttons;
    public StageState CurrentState => CurrentStageState;

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

    public void SetStageClear()
    {
        OnStageClear();
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

    public void Awake()
    {
        int stageLevel = StageManager.Instance.GetDifficulty(); // 임시 하드코딩

        buttons = new List<GameObject> { Button1, Button2, Button3, Button4 };

        // 정답 스프라이트 초기화
        Objective.GetComponent<SpriteRenderer>().sprite = stageLevel switch
        {
            1 => ObjectiveSprite1,
            2 => ObjectiveSprite2,
            3 => ObjectiveSprite3,
            4 => ObjectiveSprite4,
            _ => null
        };
        
        // 버튼 초기화
        for(int i = 0; i < 4; i++)
        {
            int RandNum = Random.Range(0, buttons.Count);
            GameObject button = buttons[RandNum];
            buttons.RemoveAt(RandNum);

            switch (stageLevel)
            {
                case 1:
                    button.GetComponent<SpriteRenderer>().sprite = i switch
                    {
                        0 => Answer1_1,
                        1 => Answer1_2,
                        2 => Answer1_3,
                        3 => Answer1_4,
                        _ => null
                    };
                    if (i == 3) button.GetComponent<DanceButton>().SetCorrect();
                    break;

                case 2:
                    button.GetComponent<SpriteRenderer>().sprite = i switch
                    {
                        0 => Answer2_1,
                        1 => Answer2_2,
                        2 => Answer2_3,
                        3 => Answer2_4,
                        _ => null
                    };
                    if (i == 0) button.GetComponent<DanceButton>().SetCorrect();
                    break;

                case 3:
                    button.GetComponent<SpriteRenderer>().sprite = i switch
                    {
                        0 => Answer3_1,
                        1 => Answer3_2,
                        2 => Answer3_3,
                        3 => Answer3_4,
                        _ => null
                    };
                    if (i == 2) button.GetComponent<DanceButton>().SetCorrect();
                    break;

                case 4:
                    button.GetComponent<SpriteRenderer>().sprite = i switch
                    {
                        0 => Answer4_1,
                        1 => Answer4_2,
                        2 => Answer4_3,
                        3 => Answer4_4,
                        _ => null
                    };
                    if (i == 0) button.GetComponent<DanceButton>().SetCorrect();
                    break;
            }
        }
    }

    void Start()
    {
        // 스테이지 시작
        OnStageStart();
        SoundManager.Instance.PlayBGM(_stageBGM);
    }

    
    void Update()
    {
        //OnStageClear();
    }
}
