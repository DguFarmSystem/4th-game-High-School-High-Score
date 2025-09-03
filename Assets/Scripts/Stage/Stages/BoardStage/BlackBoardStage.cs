using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage;

/*
 * �������� ����
 * ���� ���� : �������� ����, �������� ���� �ð� Ȯ�� ����
 * �������� ���� : �������� ���ӿ�����Ʈ�� ���������� �´� �̹��� �ο�
 * �������� ���� : �������� ���ӿ�����Ʈ�� �󸶳� ���������� Ȯ��
 */
public class BlackBoardStage : StageNormal
{
    [Header("�������� ������Ʈ")]
    [SerializeField] private GameObject BoardEraser;
    [SerializeField] private EraseTexture[] eraseTargets; //������ ������Ʈ ����

    private bool isCleared = false; //�ߺ� ������

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
                ratio += eraseTarget.ErasedRatio; //�ϴ� ��ü�� ������ ������ �׳� ���Ѵ�.
            }
            ratio /= eraseTargets.Length; //���� ��ü�� ������ŭ ������ 0~100���̷� normalize
            Debug.Log($"���� ���� ����: {ratio * 100f:F2}% / ��ǥ: {eraseTarget.erasedThreshold * 100f:F2}%");

            if (ratio >= eraseTarget.erasedThreshold - 0.0001f)
            {
                isCleared = true;
                OnStageClear();
                OnStageEnd();
            }
        }
    }
}