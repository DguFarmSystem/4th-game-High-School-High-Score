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
    AudioClip[] Audioes;//음악 소스 나오면 기획서 순서대로 음악 대입해주면 됨.
    [SerializeField]
    AudioClip buttonSFX;
    AudioSource theaudio;

    float ArrowTime = 1f;
    [SerializeField]
    GameObject Arrow;
    [SerializeField]
    GameObject[] ArrowPostion;

    float time=5f;

    // Start is called before the first frame update
    void Start()
    {
        time = 5;
        theaudio = GetComponent<AudioSource>();
        theaudio.loop = true;
        OnStageStart();
        currentmusic = Random.Range(0, 3);
        CoverImage.sprite=Covers[currentmusic];
        theaudio.clip=Audioes[currentmusic];//음악 클립 추가했으면 주석 처리 취소
        theaudio.time = 2f;
        theaudio.Play();
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
        theaudio.clip = Audioes[currentmusic];//나중에 음악 추가했으면 주석표시 지우기
        CoverImage.sprite = Covers[currentmusic];
        theaudio.time = 2f;
        theaudio.Play();
        theaudio.PlayOneShot(buttonSFX);
    }
    public void OnClickPrevious()
    {
        currentmusic--;
        if (currentmusic == -1) currentmusic = 5;
        theaudio.clip = Audioes[currentmusic];//나중에 음악 추가했으면 주석표시 지우기
        CoverImage.sprite = Covers[currentmusic];
        theaudio.time = 2f;
        theaudio.Play();
        theaudio.PlayOneShot(buttonSFX);
    }
}
