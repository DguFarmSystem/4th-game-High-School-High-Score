using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage;

public class GymBossStage : StageNormal
{
    [SerializeField]
    private BallTrack Outer;
    [SerializeField]
    private BallTrack Inner;
    [SerializeField]
    private NumberCountDown Timer;

    private float StageTimeLimit = 30.0f;
    private bool BallFallen = false;

    //public override void OnStageStart()
    //{
    //    base.OnStageStart();
    //}
    //protected override void OnStageEnd()
    //{
    //    //Debug.Log("Stage Failed.. Current Level : " + stageLevel);
    //    base.OnStageEnd();
    //}

    //protected override void OnStageClear()
    //{
    //    //stageLevel++;
    //    //Debug.Log("Stage Cleared!! Current Level : " + stageLevel);
    //    base.OnStageClear();
    //}

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

    void AddBall(float remainTime)
    {
        bool RandomWay = Random.value > 0.5f;
        if (remainTime == 24)
        {
            Outer.AddBall(6, RandomWay);
        }
        else if (remainTime == 18)
        {
            Inner.AddBall(5, RandomWay);
        }
        else if (remainTime == 8)
        {
            Outer.AddBall(2, RandomWay);
        }
    }

    void CheckifFallen()
    {
        if(Outer.getFallen() || Inner.getFallen())
        {
            //일단 실패 처리
            BallFallen = true;
            Timer.Stop = true;

            //코드 중복을 막기 위해서 조금 조건문 약간 세분화
            if (!Outer.getFallen())
            {
                Outer.DeActivateAll();
            }
            else if (!Inner.getFallen())
            {
                Inner.DeActivateAll();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Inner.AddBall(0, true);
    }

    // Update is called once per frame
    void Update()
    {
        CheckifFallen();
        if(!BallFallen)
        {
            if (StageTimeLimit > 0)
            {
                float prevTime = StageTimeLimit;
                StageTimeLimit -= Time.deltaTime;

                // 시간이 정수 단위로 바뀌었을 때만 AddBall 호출
                if ((int)prevTime != (int)StageTimeLimit)
                {
                    AddBall((int)StageTimeLimit);
                }
            }

            //스테이지 시간 종료
            else
            {
                OnStageEnded?.Invoke(!BallFallen);
            }
        }
        //공이 떨어짐
        else
        {
            OnStageEnded?.Invoke(!BallFallen);
        }
    }
}
