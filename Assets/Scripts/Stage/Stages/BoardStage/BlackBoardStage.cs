using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Stage;

/*
 * �������� ����
 * ���� ���� : �������� ����, �������� ���� �ð� Ȯ�� ����
 * �������� ���� : �������� ���ӿ�����Ʈ�� ���������� �´� �̹��� �ο�
 * �������� ���� : �������� ���ӿ�����Ʈ�� �󸶳� ���������� Ȯ��
 */
public class BlackBoardStage : StageNormal
{
    [Header("Audio Setting")]
    [SerializeField] private AudioSource EffectAudio;
    [SerializeField] private AudioClip[] EffectSound;
    [SerializeField] private AudioSource BGM_Audio;

    [Header("Stage Resource")]
    //���̵��� ������������ �ʿ��� ���ҽ��� �����ϴ� �ڵ�
    public Sprite[] backgrounds;                   //��� �̹��� ����
    public Sprite[] Erasers;                       //���찳 �̹��� ����
    [SerializeField] private Image image;          //������ ��� �̹����� ���� �̹��� ������Ʈ
    [SerializeField] private GameObject Eraser;    //���찳 �̹��� ����

    [Header("Prefab & Stage Config")]
    public EraseTexture[] erasePrefabs;   // ������ (���� �̹���)
    public Transform spawnParent;      // ������ �θ� ������Ʈ (��: Canvas�� Ư�� ��ġ)
    public float clearThreshold = 0.8f; // Ŭ���� ��ǥ (0~1 ���� ��)
    private static int stageLevel = 0;  // ���߿� StageManager���� stageLevel�� �Ѱ��ִ� ������� ���ҽ� ����

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
        BGM_Audio.Stop();
        
        if (cleared)
        {
            Debug.Log("Stage Cleared.");
            StageManager.Instance.StageClear(true);
        }
        else
        {
            Debug.Log("Stage Failed.");
            StageManager.Instance.StageClear(false);
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
        stageLevel = StageManager.Instance.GetDifficulty() - 1;
        //stageLevel = 1;
        //���, ���찳 ������Ʈ ����
        image.sprite = backgrounds[stageLevel];
        SpriteRenderer EraseRenderer = Eraser.GetComponent<SpriteRenderer>();
        EraseRenderer.sprite = Erasers[stageLevel];

        //�Ҹ� ����
        EffectAudio.clip = EffectSound[stageLevel];

        //���� ������Ʈ ����
        eraseObject = Instantiate(erasePrefabs[stageLevel], spawnParent);
        eraseObject.transform.localPosition = Vector3.zero;
        OnStageStart();
    }

    void Update()
    {
        //��ġ�� �Էµǰ� �ִ� �������� ��ġ ��ȭ
        if(InputManager.Instance.IsPressing)
        {
            Debug.Log("Board Erasing");
            //�� ƽ ���찳�� ��ġ ��ȭ
            Eraser.transform.position = InputManager.Instance.TouchWorldPos - new Vector3(1.0f, 0.0f, 0.0f);
            
            // �Ҹ��� ��� ���� �ƴ� ���� Play
            if (!EffectAudio.isPlaying)
            {
                EffectAudio.Play();
            }
        }
        else
        {
            // ��ġ�� ������ ���� Stop
            if (EffectAudio.isPlaying)
            {
                EffectAudio.Stop();
            }

        }

        float erasedRatio = eraseObject.ErasedRatio;

        // ����� ���
        //Debug.Log($"Erased Ratio: {erasedRatio * 100f:0.00}%");

        if (erasedRatio >= clearThreshold)
        {
            OnStageClear();
        }
    }
}