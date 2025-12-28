using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlaylistStage : StageNormal
{
    int currentmusic = 0;
    [SerializeField]
    Image CoverImage;
    [SerializeField]
    Sprite[] Covers;
    [SerializeField]
    AudioClip Audioes;//���� �ҽ� ������ ��ȹ�� ������� ���� �������ָ� ��.
    AudioSource theaudio;
    [SerializeField]
    AudioSource musicaudio;
    [SerializeField]
    AudioClip Ef;

    float ArrowTime = 1f;
    [SerializeField]
    GameObject Arrow;
    [SerializeField]
    GameObject[] ArrowPostion;

    float time=5f;

    bool on = false;

    // Start is called before the first frame update
    void Start()
    {
        time = 5;
        theaudio = GetComponent<AudioSource>();
        theaudio.loop = true;
        OnStageStart();
        currentmusic = Random.Range(0, 3);
        CoverImage.sprite=Covers[currentmusic];
        //theaudio.clip=Audioes[currentmusic];//���� Ŭ�� �߰������� �ּ� ó�� ���
    }

    void Update()
    {
        time -= Time.deltaTime;
        StageGimmik();
        if (time<=0)
        {
            if (currentmusic==3)
            {
                OnStageClear();
            }
        }
        if (time > 4)
        {
            switch (currentmusic)
            {
                case 0:
                    Arrow.transform.position= ArrowPostion[2].transform.position;
                    Arrow.SetActive(true);
                    break;
                case 1:
                    Arrow.transform.position = ArrowPostion[1].transform.position;
                    Arrow.SetActive(true);
                    break;
                case 2:
                    Arrow.transform.position = ArrowPostion[0].transform.position;
                    Arrow.SetActive(true);
                    break;
                default:
                    Arrow.SetActive(false);
                    break;
            }
        }
        else
        {
            Arrow.SetActive(false);
        }
        if (currentmusic == 3)
        {
            if (!on)
            {
                musicaudio.clip = Audioes;
                musicaudio.time = 2f;
                musicaudio.Play(Audioes);
                on = true;
            }

        }
        else
        {
            on = false;
            musicaudio.Stop();
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
        theaudio.Stop();
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

    }

    public void OnClickNext()
    {
        currentmusic++;
        if(currentmusic==6)currentmusic = 0;
        //theaudio.clip = Audioes[currentmusic];//���߿� ���� �߰������� �ּ�ǥ�� �����
        CoverImage.sprite = Covers[currentmusic];
        theaudio.PlayOneShot(Ef);
        
    }
    public void OnClickPrevious()
    {
        currentmusic--;
        if (currentmusic == -1) currentmusic = 5;
        //theaudio.clip = Audioes[currentmusic];//���߿� ���� �߰������� �ּ�ǥ�� �����
        CoverImage.sprite = Covers[currentmusic];
        theaudio.PlayOneShot(Ef);
    }
}
