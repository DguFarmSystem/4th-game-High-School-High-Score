using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 레거시 Text 사용
using Stage; // 다른 미니게임과 동일하게 StageManager 접근

public class Bar : MonoBehaviour
{
    [Header("References")]
    [Tooltip("50cm 위치를 나타내는 기준 오브젝트 (UI RectTransform)")]
    public Transform pos50cm;
    [Tooltip("100cm 위치를 나타내는 기준 오브젝트 (UI RectTransform)")]
    public Transform pos100cm;

    [Header("Movement Settings")]
    [Tooltip("이동을 시작하기 전까지의 대기 시간")]
    public float startDelay = 2.3f;
    [Tooltip("아래로 내려가는 속도")]
    public float moveSpeed = 2f;

    // HeightStageManager에서만 레벨을 관리하도록 Bar의 인스펙터에서는 아예 숨깁니다.
    [HideInInspector]
    public int currentLevel = 1;

    private Text heightText;
    private RectTransform myRect; // 터치 판정용

    private float timer = 0f;
    private bool isMoving = false;
    private bool isStopped = false;
    private float currentCm = 70.0f;

    void Start()
    {
        heightText = GetComponentInChildren<Text>();
        myRect = GetComponent<RectTransform>();

        // 우선적으로 씬 내의 HeightStageManager 설정(테스트 레벨 등)을 가져옵니다.
        HeightStageManager localManager = FindObjectOfType<HeightStageManager>();
        if (localManager != null)
        {
            currentLevel = localManager.GetCurrentLevel();
        }
        else if (StageManager.Instance != null)
        {
            currentLevel = Mathf.Max(1, StageManager.Instance.GetDifficulty());
        }
        
        CalculateHeight();
    }

    void Update()
    {
        if (isStopped) return;

        // 터치 및 클릭 입력 처리 (BeatStageManager 로직 참고)
        HandleInput();

        if (isStopped) return;

        timer += Time.deltaTime;

        if (timer >= startDelay && !isMoving)
        {
            isMoving = true;
        }

        if (isMoving)
        {
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
        }

        CalculateHeight();
    }

    void HandleInput()
    {
        Camera uiCamera = Camera.main;

        // 1. 모바일 터치 처리
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
                ProcessHit(touch.position, uiCamera);
            }
        }

        // 2. PC 마우스 처리
        if (Input.touchCount == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ProcessHit(Input.mousePosition, uiCamera);
            }
        }
    }

    void ProcessHit(Vector2 hitPos, Camera uiCamera)
    {
        // RectTransform이 없으면 터치 판정 불가
        if (myRect == null) return;

        // RectTransform 기준으로 터치 범위 검사 (BeatStageManager와 동일)
        if (RectTransformUtility.RectangleContainsScreenPoint(myRect, hitPos, uiCamera))
        {
            isStopped = true;
            CheckClear();
        }
    }

    void CalculateHeight()
    {
        if (pos50cm == null || pos100cm == null) return;

        float distanceUnits = pos100cm.position.y - pos50cm.position.y;
        float cmPerUnit = 50f / distanceUnits;
        currentCm = 50f + ((transform.position.y - pos50cm.position.y) * cmPerUnit);

        if (heightText != null)
        {
            heightText.text = currentCm.ToString("F1") + "cm";
        }
    }

    void CheckClear()
    {
        bool isSuccess = false;

        switch (currentLevel)
        {
            case 1:
                if (currentCm >= 27.0f && currentCm <= 33.0f) isSuccess = true;
                break;
            case 2:
                if (currentCm >= 47.0f && currentCm <= 53.0f) isSuccess = true;
                break;
            case 3:
                if (currentCm >= 57.0f && currentCm <= 63.0f) isSuccess = true;
                break;
            case 4:
                if (currentCm >= 77.0f && currentCm <= 83.0f) isSuccess = true;
                break;
        }

        if (isSuccess)
        {
            Debug.Log($"[HeightStage] 성공! 멈춘 위치: {currentCm:F1}cm (적용된 레벨: {currentLevel})");
            // StageManager 연동 (CountManager, BeatStageManager 참고)
            if (StageManager.Instance != null)
            {
                StageManager.Instance.StageClear(true);
            }
        }
        else
        {
            Debug.Log($"[HeightStage] 실패... 멈춘 위치: {currentCm:F1}cm (적용된 레벨: {currentLevel})");
            if (StageManager.Instance != null)
            {
                StageManager.Instance.StageClear(false);
            }
        }
    }
}
