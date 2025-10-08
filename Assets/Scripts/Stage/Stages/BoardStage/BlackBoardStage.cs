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
    [Header("Prefab & Stage Config")]
    public EraseTexture[] erasePrefabs;   // ������ (���� �̹���)
    public Transform spawnParent;      // ������ �θ� ������Ʈ (��: Canvas�� Ư�� ��ġ)
    public float clearThreshold = 0.8f; // Ŭ���� ��ǥ (0~1 ���� ��)
    private static int stageLevel = 1;  // ���߿� StageManager���� stageLevel�� �Ѱ��ִ� ������� ���ҽ� ����

    private EraseTexture eraseObject;  // ������ EraseTexture �ν��Ͻ�
    private float timer;
    //private bool stageEnded = false;

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

    private void StageDistinction(bool cleared)
    {
        if (cleared)
        {
            Debug.Log("Stage Cleared.");
            //Ŭ���� ����, �������� �� �߰� ������ �̵�
        }
        else
        {
            Debug.Log("Stage Failed.");
            //���� ����, �������� �� �߰� ������ �̵�
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
        //if (stageEnded) return;
        //spawnPoint ������ǥ ����(0,0)�� ��ġ 
        eraseObject = Instantiate(erasePrefabs[stageLevel], spawnParent);
        eraseObject.transform.localPosition = Vector3.zero;
        OnStageStart();
    }

    void Update()
    {
        float erasedRatio = eraseObject.ErasedRatio;

        // ����� ���
        Debug.Log($"Erased Ratio: {erasedRatio * 100f:0.00}%");

        if (erasedRatio >= clearThreshold)
        {
            OnStageClear();
        }
    }
}