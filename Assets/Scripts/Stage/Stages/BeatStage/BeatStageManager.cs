using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Stage;
using System.Reflection;

public class BeatStageManager : StageNormal
{
    public int Level = 1; // 테스트용

    [Header("악보")]
    public List<GameObject> Songs = new();
    [Header("hi, snare 오브젝트")]
    public GameObject Hi;
    public GameObject Snare;
    [Header("hi, snare 히트박스")]
    public GameObject Hihitbox;
    public GameObject NoHihitbox;
    public GameObject Snarehitbox;
    public GameObject NoSnarehitbox;
    
    [Header("오차범위")]
    public float Ocha = 0.3f;

    [Header("레벨 1")]
    public List<float> songA = new();
    [Header("레벨 2")]
    public List<float> songB = new();

    private int Hiidx = 0;
    private int Snareidx = 0;
    private List<float> CurrentSong = new();
    private List<float> HiList = new();
    private List<float> SnareList = new();
    private bool wrong = false;

    [Header("사운드")]
    public AudioClip hiClip;
    public AudioClip snareClip;
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

        Hihitbox.SetActive(false);
        Snarehitbox.SetActive(false);

        foreach(GameObject song in Songs)
            song.SetActive(false);

        Songs[Level - 1].SetActive(true);
        
        if (Level == 1) 
        {
            CurrentSong = songA;
            // 2.3초 뒤로 다 미룸
            for (int i = 0; i < CurrentSong.Count; i++)
            {
                CurrentSong[i] += 2.3f;
            }
            HiList = CurrentSong.GetRange(0, 4);
            SnareList = CurrentSong.GetRange(4, 2);
        }
        else if (Level == 2) 
        {
            CurrentSong = songB;
            // 2.3초 뒤로 다 미룸
            for (int i = 0; i < CurrentSong.Count; i++)
            {
                CurrentSong[i] += 2.3f;
            }
            HiList = CurrentSong.GetRange(0, 5);
            SnareList = CurrentSong.GetRange(5, 4);
        }
    }
    void Update()
    {
        Vector3? firstTouchPos = null;
        Vector3? secondTouchPos = null;

        bool hiTiming = (Hiidx < HiList.Count && Time.time >= HiList[Hiidx] - Ocha && Time.time <= HiList[Hiidx] + Ocha);
        bool snareTiming = (Snareidx < SnareList.Count && Time.time >= SnareList[Snareidx] - Ocha && Time.time <= SnareList[Snareidx] + Ocha);

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
                Vector3 screenPos = touch.position;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane));
                worldPos.z = 0;

                // 판정 타이밍 구간이 아닐 때 하이햇/스네어를 치면 실패
                if (!hiTiming && Hihitbox.GetComponent<Collider2D>().OverlapPoint(worldPos))
                {
                    GameObject temp1 = Instantiate(Hi);
                    Destroy(temp1, Ocha);
                    Debug.Log("하이햇 판정 외 입력 - 실패!");
                    wrong = true;
                }
                if (!snareTiming && Snarehitbox.GetComponent<Collider2D>().OverlapPoint(worldPos))
                {
                    GameObject temp2 = Instantiate(Snare);
                    Destroy(temp2, Ocha);
                    Debug.Log("스네어 판정 외 입력 - 실패!");
                    wrong = true;
                }

                if (i == 0)
                    firstTouchPos = worldPos;
                else if (i == 1)
                    secondTouchPos = worldPos;
            }
        }

        // 하이햇 판정 타이밍 구간
        if (Hiidx < HiList.Count && Time.time >= HiList[Hiidx] - Ocha && Time.time <= HiList[Hiidx] + Ocha)
        {
            bool hiHit = false;
            if (firstTouchPos.HasValue && Hihitbox.GetComponent<Collider2D>().OverlapPoint(firstTouchPos.Value))
                hiHit = true;
            if (secondTouchPos.HasValue && Hihitbox.GetComponent<Collider2D>().OverlapPoint(secondTouchPos.Value))
                hiHit = true;

            if (hiHit)
            {
                GameObject temp1 = Instantiate(Hi);
                Destroy(temp1, Ocha);
                Debug.Log("하이햇 성공!");
                Hiidx = Mathf.Min(Hiidx + 1, HiList.Count - 1);
                PlayHi();
            }
            else
            {
                Debug.Log("하이햇 미스!");
                wrong = true;
            }
        }

        // 스네어 판정 타이밍 구간
        if (Snareidx < SnareList.Count && Time.time >= SnareList[Snareidx] - Ocha && Time.time <= SnareList[Snareidx] + Ocha)
        {
            bool snareHit = false;
            if (firstTouchPos.HasValue && Snarehitbox.GetComponent<Collider2D>().OverlapPoint(firstTouchPos.Value))
                snareHit = true;
            if (secondTouchPos.HasValue && Snarehitbox.GetComponent<Collider2D>().OverlapPoint(secondTouchPos.Value))
                snareHit = true;

            if (snareHit)
            {
                GameObject temp2 = Instantiate(Snare);
                Destroy(temp2, Ocha);
                Debug.Log("스네어 성공!");
                Snareidx = Mathf.Min(Snareidx + 1, SnareList.Count - 1);
                PlaySnare();
            }
            else
            {
                Debug.Log("스네어 미스!");
                wrong = true;
            }
        }
    }

    public override void OnStageStart()
    {
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
}