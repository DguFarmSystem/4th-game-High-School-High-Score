using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage;
using System.Linq;

public class BeatStageManager : MonoBehaviour, IStageBase
{
    [Header("속도")]
    public float NoteSpeed = 5.0f; 

    [Header("기본 설정")]
    public float StartDelay = 3.3f;  
    public float FailDuration = 1.0f;
    
    [Header("오브젝트 연결")]
    public GameObject ScanLineBar;
    
    [Header("레벨 1 설정")]
    public GameObject Level1_Sheet;   
    public GameObject Level1_Parent;  
    public Transform Level1_EndPoint; 

    [Header("레벨 2 설정")]
    public GameObject Level2_Sheet;  
    public GameObject Level2_Parent;  
    public Transform Level2_EndPoint;  

    [Header("이펙트 & 사운드")]
    public GameObject HiHitEffect;    
    public GameObject SnareHitEffect; 
    public GameObject HiFailArt;      
    public GameObject SnareFailArt;   
    
    public Collider2D Hihitbox;
    public Collider2D Snarehitbox;

    public AudioClip hiClip;
    public AudioClip snareClip;
    private AudioSource audioSource;

    // === 내부 변수 ===
    private bool isGameRunning = false;
    private bool isRoundFail = false; 
    private Vector3 barStartPos; 

    private Queue<RhythmNote> hittableHiNotes = new();
    private Queue<RhythmNote> hittableSnareNotes = new();

    private Coroutine hiFailRoutine;
    private Coroutine snareFailRoutine;
    
    private GameObject hiEffectInstance;
    private GameObject snareEffectInstance;

    // Interface
    public StageState CurrentState => CurrentStageState;
    protected StageState CurrentStageState = StageState.NotStart;
    public Action<bool> OnStageEnded { get; protected set; }
    public float StageTimeLimit { get; private set; } = 0f; 

    public void OnEnable() => OnStageEnded += OnStageEndedGimmick;
    public void OnDisable() => OnStageEnded -= OnStageEndedGimmick;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        
        if(HiFailArt) HiFailArt.SetActive(false);
        if(SnareFailArt) SnareFailArt.SetActive(false);

        if (HiHitEffect && Hihitbox) {
            hiEffectInstance = Instantiate(HiHitEffect, Hihitbox.transform.position, Quaternion.identity);
            hiEffectInstance.SetActive(false);
        }
        if (SnareHitEffect && Snarehitbox) {
            snareEffectInstance = Instantiate(SnareHitEffect, Snarehitbox.transform.position, Quaternion.identity);
            snareEffectInstance.SetActive(false);
        }

        if (ScanLineBar) 
        {
            barStartPos = ScanLineBar.transform.position;
            ScanLineBar.SetActive(false);
        }
        
        if(Level1_Parent) Level1_Parent.SetActive(false);
        if(Level2_Parent) Level2_Parent.SetActive(false);
        if(Level1_Sheet) Level1_Sheet.SetActive(false);
        if(Level2_Sheet) Level2_Sheet.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(StageRoutine());
    }

    IEnumerator StageRoutine()
    {
        yield return new WaitForSeconds(2.3f);

        Debug.Log("레벨 1 시작");
        
        if(ScanLineBar) ScanLineBar.SetActive(true);
        if(Level1_Sheet) Level1_Sheet.SetActive(true);

        yield return StartCoroutine(PlayRound(Level1_Parent, Level1_EndPoint));

        // [레벨 1] 정리
        if(ScanLineBar) ScanLineBar.SetActive(false);
        if(Level1_Parent) Level1_Parent.SetActive(false);
        if(Level1_Sheet) Level1_Sheet.SetActive(false);
        
        hittableHiNotes.Clear();
        hittableSnareNotes.Clear();

        // [레벨 1] 결과 판정
        if (isRoundFail)
        {
            Debug.Log("레벨 1 실패 -> 게임 오버");
            SetStageFailed();
            yield break;
        }

        yield return new WaitForSeconds(2.3f);

        Debug.Log("레벨 2 시작");
        
        if(ScanLineBar) ScanLineBar.SetActive(true);
        if(Level2_Sheet) Level2_Sheet.SetActive(true);

        yield return StartCoroutine(PlayRound(Level2_Parent, Level2_EndPoint));

        // [레벨 2] 정리
        if(ScanLineBar) ScanLineBar.SetActive(false);
        if(Level2_Parent) Level2_Parent.SetActive(false);
        if(Level2_Sheet) Level2_Sheet.SetActive(false);

        // [레벨 2] 결과 판정
        if (isRoundFail)
        {
            Debug.Log("레벨 2 실패 -> 게임 오버");
            SetStageFailed();
        }
        else
        {
            Debug.Log("모든 스테이지 클리어");
            SetStageClear();
        }
    }

    IEnumerator PlayRound(GameObject levelParent, Transform endPoint)
    {
        if (levelParent == null || endPoint == null)
        {
            Debug.LogError("레벨 부모 혹은 EndPoint 연결 안됨");
            yield break;
        }

        levelParent.SetActive(true);
        if(ScanLineBar) ScanLineBar.transform.position = barStartPos; 
        
        isRoundFail = false; 
        isGameRunning = true;
        
        SetupPreplacedNotes(levelParent);
        OnStageStart(); 

        while (ScanLineBar.transform.position.x < endPoint.position.x)
        {
            if (CurrentStageState != StageState.Playing) yield break;
            yield return null;
        }

        isGameRunning = false;
    }

    void SetupPreplacedNotes(GameObject parent)
    {
        RhythmNote[] notes = parent.GetComponentsInChildren<RhythmNote>();
        foreach (var note in notes)
        {
            note.manager = this;
            note.gameObject.SetActive(true);
        }
    }

    public void OnStageStart()
    {
        CurrentStageState = StageState.Playing;
        isGameRunning = true;
    }

    void Update()
    {
        if (CurrentStageState != StageState.Playing) return;
        if (!isGameRunning) return;

        if (ScanLineBar)
        {
            ScanLineBar.transform.position += Vector3.right * NoteSpeed * Time.deltaTime;
        }

        HandleInput();
    }

    public void RegisterHittableNote(RhythmNote note)
    {
        if (note.type == NoteType.Hi) hittableHiNotes.Enqueue(note);
        else hittableSnareNotes.Enqueue(note);
    }

    public void UnregisterNote(RhythmNote note, bool isMiss)
    {
        if (note.type == NoteType.Hi)
        {
            if (hittableHiNotes.Count > 0 && hittableHiNotes.Peek() == note)
            {
                hittableHiNotes.Dequeue();
                if (isMiss) 
                {
                    Debug.Log("Miss (Hi)");
                    ShowFailArt(true);
                    isRoundFail = true; 
                }
            }
        }
        else
        {
            if (hittableSnareNotes.Count > 0 && hittableSnareNotes.Peek() == note)
            {
                hittableSnareNotes.Dequeue();
                if (isMiss) 
                {
                    Debug.Log("Miss (Snare)");
                    ShowFailArt(false);
                    isRoundFail = true; 
                }
            }
        }
    }

    void HandleInput()
    {
        bool isHiActive = false;
        bool isSnareActive = false;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            Vector3 wp = Camera.main.ScreenToWorldPoint(touch.position);
            wp.z = 0;

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if (Hihitbox.OverlapPoint(wp)) isHiActive = true;
                if (Snarehitbox.OverlapPoint(wp)) isSnareActive = true;
            }

            if (touch.phase == TouchPhase.Began)
            {
                if (Hihitbox.OverlapPoint(wp))
                {
                    PlayHi();
                    if (hittableHiNotes.Count > 0)
                    {
                        RhythmNote note = hittableHiNotes.Dequeue();
                        note.OnHit();
                        Debug.Log("Hi Hit Success!");
                    }
                    else
                    {
                        Debug.Log("Hi Fail (허공)");
                        ShowFailArt(true);
                        isRoundFail = true; 
                    }
                }
                else if (Snarehitbox.OverlapPoint(wp))
                {
                    PlaySnare();
                    if (hittableSnareNotes.Count > 0)
                    {
                        RhythmNote note = hittableSnareNotes.Dequeue();
                        note.OnHit();
                        Debug.Log("Snare Hit Success!");
                    }
                    else
                    {
                        Debug.Log("Snare Fail (허공)");
                        ShowFailArt(false);
                        isRoundFail = true; 
                    }
                }
            }
        }

        if (hiEffectInstance) hiEffectInstance.SetActive(isHiActive);
        if (snareEffectInstance) snareEffectInstance.SetActive(isSnareActive);
    }

    void ShowFailArt(bool isHi) 
    {
        GameObject target = isHi ? HiFailArt : SnareFailArt;
        if(target == null) return;
        
        if(isHi) { if(hiFailRoutine != null) StopCoroutine(hiFailRoutine); hiFailRoutine = StartCoroutine(FailRoutine(target)); }
        else { if(snareFailRoutine != null) StopCoroutine(snareFailRoutine); snareFailRoutine = StartCoroutine(FailRoutine(target)); }
    }

    IEnumerator FailRoutine(GameObject obj) 
    { 
        obj.SetActive(true); 
        yield return new WaitForSeconds(FailDuration); 
        obj.SetActive(false); 
    }
    
    public void PlayHi() { if(audioSource) audioSource.PlayOneShot(hiClip); }
    public void PlaySnare() { if(audioSource) audioSource.PlayOneShot(snareClip); }
    private void OnStageEndedGimmick(bool isClear) { StageManager.Instance.StageClear(isClear); }
    public void SetStageClear() { CurrentStageState = StageState.Clear; OnStageEnded?.Invoke(true); }
    public void SetStageFailed() { CurrentStageState = StageState.Over; OnStageEnded?.Invoke(false); }
}