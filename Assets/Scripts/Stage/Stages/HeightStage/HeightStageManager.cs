using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Stage; // StageNormal 및 StageManager 사용

public class HeightStageManager : StageNormal
{
    [Header("Test Settings")]
    [Tooltip("에디터 테스트용: 체크 시 메인 매니저를 무시하고 Test Level을 적용합니다.")]
    public bool useTestLevel = false;
    [Range(1, 4)]
    public int testLevel = 1;

    [Header("Background Settings")]
    [Tooltip("레벨별 배경 오브젝트 (인덱스 0 = 레벨 1, 인덱스 1 = 레벨 2...)")]
    public GameObject[] levelBackgrounds;

    [Header("Stage Objects")]
    [Tooltip("레벨별 Bar 오브젝트 (인덱스 0 = 레벨 1, 인덱스 1 = 레벨 2...)")]
    public GameObject[] levelBars;

    public void OnEnable()
    {
        // StageNormal의 이벤트 연결
        OnStageEnded += OnStageEndedGimmik;
    }

    public void OnDisable()
    {
        OnStageEnded -= OnStageEndedGimmik;
    }

    private void OnStageEndedGimmik(bool isStageCleared)
    {
        if (isStageCleared)
        {
            Debug.Log("[HeightStage] Stage cleared!");
        }
        else
        {
            Debug.Log("[HeightStage] Stage failed!");
        }
    }

    void Start()
    {
        // 씬 시작 시 바로 재생
        if (CurrentStageState == StageState.NotStart)
        {
            OnStageStart();
        }
    }

    // Bar.cs에서도 난이도를 일치시키기 위해 퍼블릭 함수로 제공
    public int GetCurrentLevel()
    {
        if (useTestLevel) return testLevel;
        if (StageManager.Instance != null) return Mathf.Max(1, StageManager.Instance.GetDifficulty());
        return testLevel; // StageManager가 없는 단일 씬 테스트 환경일 때도 testLevel 적용
    }

    public override void OnStageStart()
    {
        // 수동으로 상태 변경 (base.OnStageStart()의 타이머 기능 무시)
        CurrentStageState = StageState.Playing;

        // 1. 매니저에서 난이도(레벨) 가져오기
        int level = GetCurrentLevel();

        // 2. 레벨에 맞는 배경만 활성화
        for (int i = 0; i < levelBackgrounds.Length; i++)
        {
            if (levelBackgrounds[i] != null)
            {
                // 레벨은 1부터 시작하므로 인덱스는 level - 1
                levelBackgrounds[i].SetActive(i == (level - 1));
            }
        }

        // 3. 게임 플레이 오브젝트(Bar) 활성화
        for (int i = 0; i < levelBars.Length; i++)
        {
            if (levelBars[i] != null)
            {
                levelBars[i].SetActive(i == (level - 1));
            }
        }
    }
}
