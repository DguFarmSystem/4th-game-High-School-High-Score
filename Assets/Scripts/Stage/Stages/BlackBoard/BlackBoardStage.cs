using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage;

/*
 * 스테이지 설계
 * 변수 선언 : 스테이지 변수, 스테이지 지속 시간 확인 변수
 * 스테이지 시작 : 지워야할 게임오브젝트에 스테이지에 맞는 이미지 부여
 * 스테이지 도중 : 지워야할 게임오브젝트가 얼마나 지워졌는지 확인
 */
public class BlackBoardStage : StageNormal
{
    [Header("스테이지 오브젝트")]
    [SerializeField] private GameObject BoardEraser;
    [SerializeField] private EraseTexture eraseTarget; //지워질 오브젝트 연결

    public GameObject TestSphere;

    private bool isCleared = false; //중복 방지용

    public override void OnStageStart()
    {
        base.OnStageStart();
        isCleared = false;
    }

    protected override void OnStageEnd()
    {
        base.OnStageEnd();
    }

    protected override void OnStageClear()
    {
        base.OnStageClear();
    }

    private void StageDistinction(bool cleared)
    {
        if (cleared)
        {
            Debug.Log("Stage Cleared.");
        }
        else
        {
            Debug.Log("Stage Failed.");
        }
    }

    private void OnEnable()
    {
        OnStageEnded += StageDistinction;
    }

    private void OnDisable()
    {
        OnStageEnded -= StageDistinction;
    }

    void Start()
    {
        OnStageStart();
    }

    void Update()
    {
        //매 프레임 체크하여 클리어 조건 만족 시 처리
        if (!isCleared && eraseTarget != null && eraseTarget.IsFullyErased)
        {
            isCleared = true;
            OnStageClear();     // StageNormal 기반 함수
            OnStageEnd();       // 종료 처리
        }
    }
}
