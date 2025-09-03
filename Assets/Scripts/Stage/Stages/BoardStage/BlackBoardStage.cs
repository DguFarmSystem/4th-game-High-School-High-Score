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
    [SerializeField] private EraseTexture[] eraseTargets; //지워질 오브젝트 연결

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
        foreach(EraseTexture eraseTarget in eraseTargets)
        {
            float ratio = 0.0f;
            if (!isCleared && eraseTarget != null)
            {
                ratio += eraseTarget.ErasedRatio; //일단 객체별 지워진 정도를 그냥 더한다.
            }
            ratio /= eraseTargets.Length; //지울 객체의 개수만큼 나눠서 0~100사이로 normalize
            Debug.Log($"현재 지운 비율: {ratio * 100f:F2}% / 목표: {eraseTarget.erasedThreshold * 100f:F2}%");

            if (ratio >= eraseTarget.erasedThreshold - 0.0001f)
            {
                isCleared = true;
                OnStageClear();
                OnStageEnd();
            }
        }
    }
}