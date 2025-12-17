using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Stage;

/*
 * 스테이지 설계
 * 변수 선언 : 스테이지 변수, 스테이지 지속 시간 확인 변수
 * 스테이지 시작 : 지워야할 게임오브젝트에 스테이지에 맞는 이미지 부여
 * 스테이지 도중 : 지워야할 게임오브젝트가 얼마나 지워졌는지 확인
 */
public class BlackBoardStage : StageNormal
{
    [Header("Audio Setting")]
    [SerializeField] private AudioSource EffectAudio;
    [SerializeField] private AudioClip[] EffectSound;

    [Header("Stage Resource")]
    //난이도별 스테이지에서 필요한 리소스를 지정하는 코드
    public Sprite[] backgrounds;                   //배경 이미지 설정
    public Sprite[] Erasers;                       //지우개 이미지 설정
    [SerializeField] private Image image;          //선택한 배경 이미지를 넣을 이미지 오브젝트
    [SerializeField] private GameObject Eraser;    //지우개 이미지 지정

    [Header("Prefab & Stage Config")]
    public EraseTexture[] erasePrefabs;   // 프리팹 (지울 이미지)
    public Transform spawnParent;      // 스폰할 부모 오브젝트 (예: Canvas나 특정 위치)
    public float clearThreshold = 0.8f; // 클리어 목표 (0~1 사이 값)
    private static int stageLevel = 0;  // 나중에 StageManager에서 stageLevel을 넘겨주는 방식으로 리소스 지정

    private EraseTexture eraseObject;  // 스폰된 EraseTexture 인스턴스
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
            //클리어 연출, 스테이지 간 중간 레벨로 이동
        }
        else
        {
            Debug.Log("Stage Failed.");
            //실패 연출, 스테이지 간 중간 레벨로 이동
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
        //spawnPoint 월드좌표 기준(0,0)에 배치 
        stageLevel = StageManager.Instance.GetDifficulty() - 1;
        //stageLevel = 2;
        //배경, 지우개 오브젝트 설정
        image.sprite = backgrounds[stageLevel];
        SpriteRenderer EraseRenderer = Eraser.GetComponent<SpriteRenderer>();
        EraseRenderer.sprite = Erasers[stageLevel];

        //소리 설정
        EffectAudio.clip = EffectSound[stageLevel];

        //지울 오브젝트 생성
        eraseObject = Instantiate(erasePrefabs[stageLevel], spawnParent);
        eraseObject.transform.localPosition = Vector3.zero;
        OnStageStart();
    }

    void Update()
    {
        //터치가 입력되고 있는 순간에만 위치 변화
        if(InputManager.Instance.IsPressing)
        {
            Debug.Log("Board Erasing");
            //매 틱 지우개의 위치 변화
            Eraser.transform.position = InputManager.Instance.TouchWorldPos - new Vector3(1.0f, 0.0f, 0.0f);
            
            // 소리가 재생 중이 아닐 때만 Play
            if (!EffectAudio.isPlaying)
            {
                EffectAudio.Play();
            }
        }
        else
        {
            // 터치를 떼었을 때만 Stop
            if (EffectAudio.isPlaying)
            {
                EffectAudio.Stop();
            }

        }

        float erasedRatio = eraseObject.ErasedRatio;

        // 디버그 출력
        //Debug.Log($"Erased Ratio: {erasedRatio * 100f:0.00}%");

        if (erasedRatio >= clearThreshold)
        {
            OnStageClear();
        }
    }
}