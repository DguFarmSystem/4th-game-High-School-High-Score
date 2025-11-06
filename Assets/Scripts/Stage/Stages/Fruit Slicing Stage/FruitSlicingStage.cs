using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;
using UnityEngine.InputSystem;

public class FruitSlicingStage : StageNormal
{

    int GoalCount;
    int SliceCount;
    int OpCount;
    int CurrentCount=0;
    float time;
    float consttime=0.73f;

    [SerializeField]
    GameObject[] Fruits;
    [SerializeField]
    GameObject[] Checks;
    [SerializeField]
    GameObject[] GreenChecks;
    [SerializeField]
    GameObject Effect;

    int LorR;
    int Fruitnum;
    float SpawnY;

    // Start is called before the first frame update
    void Start()
    {
        time = consttime;
        CurrentCount = 0;
        int stagelevel = StageManager.Instance.GetDifficulty();
        switch (stagelevel)
        {
            case 1:
                GoalCount = 2;
                OpCount = 3;
                break;
            case 2:
                GoalCount = 3;
                OpCount= 4;
                break;
            case 3:
                GoalCount = 4;
                OpCount = 5;
                break;
            case 4:
                GoalCount = 5;
                OpCount = 6;
                break;
        }
        OnStageStart();
        for (int i = 0; i < GoalCount; i++)
        {
            Checks[i].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        StageGimmik();
        if (CurrentCount == OpCount)
        {
            if (SliceCount >= GoalCount)
            {
                OnStageClear();
            }
        }
        else
        {
            time -= Time.deltaTime;
            if(time < 0)
            {
                Fruitnum = Random.Range(0, 4);
                LorR=Random.Range(0, 2);
                SpawnY = Random.Range(-2f, 2f);
                if (LorR == 0)
                {
                    Instantiate(Fruits[Fruitnum], new Vector3(-12, SpawnY, 0), Fruits[Fruitnum].transform.rotation);
                }
                else
                {
                    Instantiate(Fruits[Fruitnum], new Vector3(12, SpawnY, 0), Fruits[Fruitnum].transform.rotation);
                }
                CurrentCount++;
                time =consttime;
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
        InputManager.Instance.OnStageTapPerformed -= StageGimmik;
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
        InputManager.Instance.OnStageTapPerformed += StageGimmik;
        OnStageEnded += OnStageEndedGimmik;
    }
    public void OnDisable()
    {
        InputManager.Instance.OnStageTapPerformed -= StageGimmik;
        OnStageEnded -= OnStageEndedGimmik;
    }

    private void StageGimmik()
    {
        Debug.Log(InputManager.Instance.TouchWorldPos);
        if (InputManager.Instance.TouchedCollider != null)
        {
            if (InputManager.Instance.TouchedCollider.gameObject.tag == "Fruit")
            {
                //Instantiate(Effect, InputManager.Instance.TouchWorldPos, Effect.transform.rotation);
                InputManager.Instance.TouchedCollider.gameObject.tag = "Untagged";
                GreenChecks[SliceCount].SetActive(true);
                SliceCount++;
            }
        }
    }
}
