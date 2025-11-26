using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage;

public class BeatStageManager : StageNormal
{
    [Header("설정")]
    public float Ocha = 0.3f; // 판정 범위 (+- 0.3초)
    public float StartDelay = 3.3f; // 시작 딜레이

    [Header("오브젝트 연결")]
    public GameObject HiHitEffect;    // 하이햇 이펙트 (초록)
    public GameObject SnareHitEffect; // 스네어 이펙트 (주황)
    public GameObject HiFailArt;      // 하이햇 Fail 이미지
    public GameObject SnareFailArt;   // 스네어 Fail 이미지
    
    [Header("히트박스")]
    public Collider2D Hihitbox;
    public Collider2D Snarehitbox;

    [Header("라운드 데이터")]
    // 각 라운드 별 노트 시간 데이터 (Inspector에서 입력)
    public List<float> Round1_Hi = new();
    public List<float> Round1_Snare = new();
    
    public List<float> Round2_Hi = new();
    public List<float> Round2_Snare = new();

    [Header("노트 오브젝트 그룹 (활성/비활성용)")]
    public GameObject Round1_Objects;
    public GameObject Round2_Objects;

    [Header("사운드")]
    public AudioClip hiClip;
    public AudioClip snareClip;
    private AudioSource audioSource;

    // 내부 상태 변수
    private List<float> currentHiList;
    private List<float> currentSnareList;
    
    private int hiIndex = 0;
    private int snareIndex = 0;
    
    private bool isRoundFail = false; // 현재 라운드에서 하나라도 틀렸는지
    private bool isGameRunning = false;
    private float gameStartTime; // 게임(라운드) 시작 시간

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        
        // 초기화
        if(HiFailArt) HiFailArt.SetActive(false);
        if(SnareFailArt) SnareFailArt.SetActive(false);
    }

    void Start()
    {
        // RestaurantBossStage와 유사한 딜레이 스타트 패턴 적용
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(StartDelay);
        OnStageStart();
    }

    public override void OnStageStart()
    {
        CurrentStageState = StageState.Playing;
        StartCoroutine(StageFlowRoutine());
    }

    // 전체 스테이지 흐름 관리 (1라운드 -> 2라운드)
    IEnumerator StageFlowRoutine()
    {
        // === 1라운드 시작 ===
        Debug.Log("1라운드 시작");
        if(Round1_Objects) Round1_Objects.SetActive(true);
        if(Round2_Objects) Round2_Objects.SetActive(false);
        
        yield return StartCoroutine(PlayRound(Round1_Hi, Round1_Snare));

        // 1라운드 종료 후 처리
        if (Round1_Objects) Round1_Objects.SetActive(false);

        if (isRoundFail)
        {
            Debug.Log("1라운드 실패 -> 스테이지 종료");
            OnStageFail();
            yield break; // 2라운드 안 감
        }
        else
        {
            Debug.Log("다음 레벨(라운드) 조건 달성: 1라운드 클리어, 2라운드로 이동합니다.");
        }

        yield return new WaitForSeconds(1f); // 라운드 사이 잠시 대기

        // === 2라운드 시작 ===
        Debug.Log("2라운드 시작");
        if(Round2_Objects) Round2_Objects.SetActive(true);

        yield return StartCoroutine(PlayRound(Round2_Hi, Round2_Snare));

        if (Round2_Objects) Round2_Objects.SetActive(false);

        if (isRoundFail)
        {
            Debug.Log("2라운드 실패 -> 스테이지 종료");
            OnStageFail();
        }
        else
        {
            Debug.Log("ALL CLEAR: 스테이지 클리어 조건 달성");
            OnStageClear();
        }
    }

    // 개별 라운드 플레이 로직
    IEnumerator PlayRound(List<float> hiData, List<float> snareData)
    {
        // 데이터 세팅
        currentHiList = new List<float>(hiData); // 복사해서 사용
        currentSnareList = new List<float>(snareData);
        
        // 딜레이 적용
        for(int i=0; i<currentHiList.Count; i++) currentHiList[i] += StartDelay;
        for(int i=0; i<currentSnareList.Count; i++) currentSnareList[i] += StartDelay;

        hiIndex = 0;
        snareIndex = 0;
        isRoundFail = false;
        
        gameStartTime = Time.time;
        isGameRunning = true;

        // 마지막 노트가 끝날 때까지 대기 (마지막 시간 + 여유시간)
        float lastTime = 0f;
        if(currentHiList.Count > 0) lastTime = Mathf.Max(lastTime, currentHiList[currentHiList.Count - 1]);
        if(currentSnareList.Count > 0) lastTime = Mathf.Max(lastTime, currentSnareList[currentSnareList.Count - 1]);
        
        float endTime = gameStartTime + lastTime + StartDelay + 1f;

        while (Time.time < endTime)
        {
            // 라운드 진행 중 업데이트
            // 코루틴 내에서 실행하므로 Update() 대신 여기서 처리하거나
            // Update()에서 isGameRunning 플래그를 보고 처리하게 할 수 있음.
            // 여기서는 Update() 함수와직 역할을 분담하기 위해 yield return null 사용.
            yield return null; 
        }

        isGameRunning = false;
    }

    void Update()
    {
        if (!isGameRunning) return;
        if (CurrentStageState != StageState.Playing) return;

        float currentTime = Time.time - gameStartTime; // (필요시 상대 시간 사용, 여기선 Time.time 절대시간 기준 유지)

        // 1. 입력 처리 (터치 시 이펙트 & 판정)
        HandleInput();

        // 2. 놓침(Miss) 처리 (시간 경과 확인)
        CheckMiss();
    }

    void HandleInput()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
                worldPos.z = 0;

                // 하이햇 터치
                if (Hihitbox.OverlapPoint(worldPos))
                {
                    ShowEffect(HiHitEffect, Hihitbox.transform.position); // 무조건 이펙트 표시
                    PlayHi();
                    
                    if (CheckTiming(currentHiList, ref hiIndex))
                    {
                        Debug.Log("하이햇 성공!");
                    }
                    else
                    {
                        Debug.Log("하이햇 타이밍/순서 틀림 (Fail)");
                        ShowFailArt(true); // 하이햇 쪽에 Fail
                        isRoundFail = true;
                    }
                }
                // 스네어 터치
                else if (Snarehitbox.OverlapPoint(worldPos))
                {
                    ShowEffect(SnareHitEffect, Snarehitbox.transform.position); // 무조건 이펙트 표시
                    PlaySnare();

                    if (CheckTiming(currentSnareList, ref snareIndex))
                    {
                        Debug.Log("스네어 성공!");
                    }
                    else
                    {
                        Debug.Log("스네어 타이밍/순서 틀림 (Fail)");
                        ShowFailArt(false); // 스네어 쪽에 Fail
                        isRoundFail = true;
                    }
                }
            }
        }
    }

    // 판정 함수: 현재 인덱스의 노트가 범위 안에 있는지 확인
    bool CheckTiming(List<float> noteList, ref int index)
    {
        if (index >= noteList.Count) return false; // 남은 노트 없음

        float targetTime = gameStartTime + noteList[index]; // 절대 시간으로 변환이 필요하면 여기서 보정
        // 위 PlayRound에서 이미 StartDelay를 더했으므로 Time.time과 비교 시 주의.
        // PlayRound에서 리스트 값 자체를 Time.time 기준으로 변환하는 것이 복잡하면
        // 차라리 StartTime을 0으로 잡고 로직을 짤 수도 있으나, 기존 구조를 살려 비교합니다.
        
        // 주의: PlayRound에서 리스트값에 StartDelay를 더했지만, 기준점(gameStartTime)이 Time.time임.
        // 리스트의 값들이 "시작 후 n초"라면: targetTime = gameStartTime + List[i]
        // 리스트의 값들이 "절대 시간"이라면 그대로 사용.
        // 기존 코드 로직을 따라 "리스트 값" 자체가 시간이라고 가정하고 비교합니다.
        
        float noteTime = noteList[index]; // PlayRound에서 이미 값이 조정됨(가정)

        if (Time.time >= noteTime - Ocha && Time.time <= noteTime + Ocha)
        {
            index++; // 다음 노트로 넘어감
            return true;
        }
        return false;
    }

    void CheckMiss()
    {
        // 하이햇 놓침 체크
        if (hiIndex < currentHiList.Count)
        {
            if (Time.time > currentHiList[hiIndex] + Ocha)
            {
                Debug.Log("하이햇 놓침! (Fail)");
                ShowFailArt(true);
                isRoundFail = true;
                hiIndex++; // 다음 노트로 강제 이동
            }
        }

        // 스네어 놓침 체크
        if (snareIndex < currentSnareList.Count)
        {
            if (Time.time > currentSnareList[snareIndex] + Ocha)
            {
                Debug.Log("스네어 놓침! (Fail)");
                ShowFailArt(false);
                isRoundFail = true;
                snareIndex++; // 다음 노트로 강제 이동
            }
        }
    }

    // 이펙트 표시 헬퍼
    void ShowEffect(GameObject effectPrefab, Vector3 pos)
    {
        if(effectPrefab)
        {
            GameObject obj = Instantiate(effectPrefab, pos, Quaternion.identity);
            Destroy(obj, 0.5f);
        }
    }

    // Fail 아트 표시 코루틴
    void ShowFailArt(bool isHi)
    {
        StartCoroutine(FailArtRoutine(isHi ? HiFailArt : SnareFailArt));
    }

    IEnumerator FailArtRoutine(GameObject artObj)
    {
        if (artObj == null) yield break;
        
        artObj.SetActive(true);
        yield return new WaitForSeconds(1.0f); // 1초간 표시
        artObj.SetActive(false);
    }
    
    // ... OnStageEnd, OnStageClear 구현 (기존 유지 혹은 부모 호출) ...
    protected override void OnStageEnd()
    {
        // 실패 시 HP 깎는 로직 등
        Debug.Log("OnStageEnd: 스테이지 실패 핸들러 호출");
        base.OnStageEnd(); 
        StageManager.Instance.StageClear(false);
    }

    protected override void OnStageClear()
    {
        Debug.Log("OnStageClear: 스테이지 클리어 핸들러 호출");
        base.OnStageClear();
        StageManager.Instance.StageClear(true);
    }
    
    private void OnStageFail()
    {
        Debug.Log("OnStageFail: 스테이지 실패 상태로 전환");
        CurrentStageState = StageState.Over;
        OnStageEnd();
    }
    
    // PlayHi, PlaySnare 등 유지
    public void PlayHi() { if(audioSource) audioSource.PlayOneShot(hiClip); }
    public void PlaySnare() { if(audioSource) audioSource.PlayOneShot(snareClip); }
}