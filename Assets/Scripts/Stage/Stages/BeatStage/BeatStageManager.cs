using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Stage;
using System.Reflection;

[Serializable]
public class BeatStageElement
{
    public float time;
    public GameObject target;
}

public class BeatStageManager : StageNormal
{
    [HideInInspector]
    public int rightcount = 0; // 2 3 8 5
    private int Lvrightcount;
    public int Level = 1; // 테스트용
    [Header("오차범위")]
    public float Ocha = 0.3f;

    [Header("악보")]
    public List<GameObject> Songs = new();

    [Header("레벨 1")]
    public List<BeatStageElement> songA = new();
    [Header("레벨 2")]
    public List<BeatStageElement> songB = new();
    [Header("레벨 3")]
    public List<BeatStageElement> songC = new();
    [Header("레벨 4")]
    public List<BeatStageElement> songD = new();
    
    [Header("삼각형")]
    public List<GameObject> Triangle = new();

    private int Hiidx = 0;
    private int Snareidx = 0;
    private int Bassidx = 0;
    private int Triidx = 0;
    private List<BeatStageElement> CurrentSong = new();
    private List<BeatStageElement> HiList = new();
    private List<BeatStageElement> SnareList = new();
    private List<BeatStageElement> BassList = new();
    private bool done = false;
    private bool wrong = false;
    [Header("사운드")]
    public AudioClip hiClip;
    public AudioClip snareClip;
    public AudioClip bassClip;
    private AudioSource audioSource;
    void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    void Start()
    {
        // Level = StageManager.Instance.GetDifficulty(); 테스트 끝나면

        foreach(GameObject song in Songs)
            song.SetActive(false);

        Songs[Level - 1].SetActive(true);
        
        foreach(GameObject tri in Triangle)
            tri.SetActive(false);
        
        if (Level == 1) 
        {
            CurrentSong = songA;
            foreach (BeatStageElement t in CurrentSong)
            {
                t.time += 2.3f;
            }
            HiList = CurrentSong.GetRange(0, 8);
            SnareList = CurrentSong.GetRange(8, 2);
            BassList = CurrentSong.GetRange(10, 2);
            Lvrightcount = 2;
        }
        else if (Level == 2) 
        {
            CurrentSong = songB;
            foreach (BeatStageElement t in CurrentSong)
            {
                t.time += 2.3f;
            }
            HiList = CurrentSong.GetRange(0, 8);
            SnareList = CurrentSong.GetRange(8, 2);
            BassList = CurrentSong.GetRange(10, 3);
            Lvrightcount = 3;
        }
        else if (Level == 3) 
        {
            CurrentSong = songC;
            foreach (BeatStageElement t in CurrentSong)
            {
                t.time += 2.3f;
            }
            HiList = CurrentSong.GetRange(0, 8);
            SnareList = CurrentSong.GetRange(8, 2);
            BassList = CurrentSong.GetRange(10, 4);
            Lvrightcount = 8;
        }
        else if (Level == 4) 
        {
            CurrentSong = songD;
            foreach (BeatStageElement t in CurrentSong)
            {
                t.time += 2.3f;
            }
            HiList = CurrentSong.GetRange(0, 8);
            SnareList = CurrentSong.GetRange(8, 2);
            BassList = CurrentSong.GetRange(10, 3); 
            Lvrightcount = 5;
        }

        Invoke(nameof(OnStageStart), 2.3f);
    }
    void Update()
    {
        if (Triidx < 8)
        {
            if (Hiidx < HiList.Count && Time.time >= HiList[Hiidx].time - Ocha)
            {
                Invoke("PlayHi", Ocha);
                // 삼각형 소환
                Triangle[Triidx].SetActive(true);
                Destroy(Triangle[Triidx++], 2 * Ocha);
                GameObject Hihat = Instantiate(HiList[Hiidx++].target);
                Destroy(Hihat, 2 * Ocha);
            }
            if (Snareidx < SnareList.Count && Time.time >= SnareList[Snareidx].time - Ocha)
            {
                Invoke("PlaySnare", Ocha);
                GameObject Snare = Instantiate(SnareList[Snareidx++].target);
                Destroy(Snare, 2 * Ocha);
            }
            if (Bassidx < BassList.Count && Time.time >= BassList[Bassidx].time - Ocha)
            {
                Invoke("PlayBass", Ocha);
                GameObject Bass = Instantiate(BassList[Bassidx++].target);
                Destroy(Bass, 2 * Ocha);
            }
        }
        else
        {
            if (done == false)
            {
                if (rightcount == Lvrightcount)
                {
                    // 모든 카운트가 맞으면 스테이지 클리어 처리
                    CurrentStageState = StageState.Clear;
                    OnStageClear();
                }
                else if (wrong == true)
                {
                    Debug.Log("잘못 쳐서 실패");
                    CurrentStageState = StageState.Over;
                    OnStageEnd();
                }
                else
                {
                    Debug.Log("뭔가를 치지 못해서 실패");
                    CurrentStageState = StageState.Over;
                    OnStageEnd();
                }
            }
            done = true;
        }
    }

    public override void OnStageStart()
    {
        base.OnStageStart(); // StageNormal의 타이머 시작
        CurrentStageState = StageState.Playing;
    }

    protected override void OnStageClear() //성공시
    {
        base.OnStageClear();
    }

    protected override void OnStageEnd() // 실패시
    {
        base.OnStageEnd();
    }

    // 일단은 디버그 로그에만 출력
    private void OnStageEndedGimmik(bool isStageCleared)
    {
        if (isStageCleared) // 성공 기믹 (아직까진 로그에)
        {
            Debug.Log("성공!");
            StageManager.Instance.StageClear(true);
        }
        else // 실패 기믹
        {
            Debug.Log("실패!");
            StageManager.Instance.StageClear(false);
        }
    }

    public void OnEnable()
    {
        OnStageEnded += OnStageEndedGimmik;
    }
    
    public void OnDisable()
    {
        OnStageEnded -= OnStageEndedGimmik;
    }

    public void PlayHi() {
        audioSource.PlayOneShot(hiClip);
    }
    public void PlaySnare() {
        audioSource.PlayOneShot(snareClip);
    }
    public void PlayBass() {
        audioSource.PlayOneShot(bassClip);
    }
}