using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Ball
{
    public int BallIndex = 0;
    public bool RightWard = true;
    public float Timer = 0.0f;
        
    Ball()
    {

    }

    public Ball(int index, bool dir)
    {
        BallIndex = index;
        RightWard = dir;
        Timer = 0.0f;
    }

    ~Ball()
    {

    }
}

public class BallTrack : MonoBehaviour
{
    public Image[] TrackLine;
    public Image[] FallenBalls;
    public RobotControl RobotState;
    public List<Ball> ActiveBall = new List<Ball>();

    public AudioSource TrackAudio;
    public AudioClip BallMovingSound;
    public AudioClip BallCrushSound;
    public event Action<Ball> OnCheckFallLeft;
    public event Action<Ball> OnCheckFallRight;

    private float deltaBallMoving = 0.9f;
    private float Tempdelta = 0.0f;
    private bool Fallen = false;


    public bool getFallen()
    {
        return Fallen;
    }

    public void SetdeltaBallMoving(float BallSpeed)
    {
        deltaBallMoving = BallSpeed;
    }

    public void AddBall(int index, bool Rightward)
    {
        Ball NewBall = new Ball(index, Rightward);
        ActiveBall.Add(NewBall);
    }

    public void DeActivateAll()
    {
        foreach(Ball ball in ActiveBall)
        {
            TrackLine[ball.BallIndex].gameObject.SetActive(false);
        }
        Fallen = true;
    }

    void TrackRunning(int ballIndex)
    {
        // 리스트에서 해당 공 객체를 가져옴
        Ball ball = ActiveBall[ballIndex];

        if (ball.Timer < 0.0f)
        {
            //변경 전 위치 기억
            int currentIndex = ball.BallIndex;

            // 참조 변수를 사용해 코드 가독성 높이기
            // 클래스(Ball)는 참조 타입이므로 ref 키워드 없이도 내부 값이 수정됩니다.
            if (ball.RightWard)
            {
                //TrackLine[ball.BallIndex].gameObject.SetActive(false);
                ball.BallIndex++;
                TrackLine[ball.BallIndex].gameObject.SetActive(true);

                if (ball.BallIndex == TrackLine.Length - 1)
                {
                    OnCheckFallRight?.Invoke(ball);
                    ball.RightWard = false;
                }
            }
            else
            {
                //TrackLine[ball.BallIndex].gameObject.SetActive(false);
                ball.BallIndex--;
                TrackLine[ball.BallIndex].gameObject.SetActive(true);

                if (ball.BallIndex == 0)
                {
                    OnCheckFallLeft?.Invoke(ball);
                    ball.RightWard = true;
                }
            }

            // [핵심] 내가 떠난 'currentIndex' 자리에 다른 공이 있는지 확인합니다.
            // 모든 활성 공을 전수조사하여, 내 BallIndex와 다른 공들 중 currentIndex에 있는 공이 있는지 체크
            bool isAnotherBallThere = false;
            foreach (Ball otherBall in ActiveBall)
            {
                if (otherBall != ball && otherBall.BallIndex == currentIndex)
                {
                    isAnotherBallThere = true;
                    break;
                }
            }

            // 다른 공이 없을 때만 이전 칸을 끕니다.
            if (!isAnotherBallThere)
            {
                TrackLine[currentIndex].gameObject.SetActive(false);
            }

            // 해당 공의 타이머만 리셋!
            ball.Timer = deltaBallMoving;

            //공 이동 사운드 출력
            TrackAudio.clip = BallMovingSound;
            TrackAudio.Play();
        }
        else
        {
            // 해당 공의 타이머만 줄임
            ball.Timer -= Time.deltaTime;
        }
    }

    IEnumerator CheckLeftTilt(Ball checkBall, int FallImageIndex)
    {
        float timer = 0.9f;

        while (timer > 0f)
        {
            if (checkBall == null)
                yield break;

            //check if it is LeftTilted
            if (RobotState.LeftTilted)
                yield break;

            timer -= Time.deltaTime;
            yield return null;
        }

        //나머지 볼 다 끄고, 게임 기능 멈추기
        DeActivateAll();
        FallenBalls[FallImageIndex].gameObject.SetActive(true);
        Fallen = true;
        TrackAudio.clip = BallCrushSound;
        TrackAudio.Play();
    }

    IEnumerator CheckRightTilt(Ball checkBall, int FallImageIndex)
    {
        float timer = 0.9f;

        while (timer > 0f)
        {
            if (checkBall == null)
                yield break;

            //check if it is RightTilted
            if (!RobotState.LeftTilted)
                yield break;

            timer -= Time.deltaTime;
            yield return null;
        }

        //나머지 볼 다 끄고, 게임 기능 멈추기
        DeActivateAll();
        FallenBalls[FallImageIndex].gameObject.SetActive(true);
        Fallen = true;
        TrackAudio.clip = BallCrushSound;
        TrackAudio.Play();
    }

    void CheckBallFall_LeftSide(Ball Checkball)
    {
        //outer ball fall check
        if(TrackLine.Length == 11)
        {
            StartCoroutine(CheckLeftTilt(Checkball, 0));
        }

        //inner ball fall check
        else
        {
            StartCoroutine(CheckRightTilt(Checkball, 0));
        }
    }

    void CheckBallFall_RightSide(Ball Checkball)
    {
        //outer ball fall check
        if (TrackLine.Length == 11)
        {
            StartCoroutine(CheckRightTilt(Checkball, 1));
        }

        //inner ball fall check
        else
        {
            StartCoroutine(CheckLeftTilt(Checkball, 1));
        }
    }
    //delegate 등록/해제
    private void OnEnable()
    {
        OnCheckFallLeft += CheckBallFall_LeftSide;
        OnCheckFallRight += CheckBallFall_RightSide;
    }

    private void OnDisable()
    {
        OnCheckFallLeft -= CheckBallFall_LeftSide;
        OnCheckFallRight -= CheckBallFall_RightSide;
    }
    // Start is called before the first frame update
    void Start()
    {
        RobotState = FindObjectOfType<RobotControl>();
        TrackAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!Fallen)
        {
            for (int i = 0; i < ActiveBall.Count; ++i)
            {
                TrackRunning(i);
            }
        }
    }
}
