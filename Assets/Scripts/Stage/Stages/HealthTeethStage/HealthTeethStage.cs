using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;
using UnityEngine.UI;

public class HealthTeethStage : StageNormal
{
    [Header("GameObject Settings")]
    [SerializeField] private GameObject _objectiveSpawner;
    [SerializeField] private GameObject _backgroundGO;

    [Space(10)]
    [Header("Lv1 Settings")]
    [SerializeField] private Sprite _backgroundLv1;

    [Space(10)]
    [Header("Lv2 Settings")]
    [SerializeField] private Sprite _backgroundLv2;

    [Space(10)]
    [Header("Lv3 Settings")]
    [SerializeField] private Sprite _backgroundLv3;

    [Space(10)]
    [Header("Lv4 Settings")]
    [SerializeField] private Sprite _backgroundLv4;
    
    [Space(10)]
    [Header("BGM Settings")]
    [SerializeField] private AudioClip _stageBGM;


    private void StageGimmikTap()
    {
        Collider2D tappedCollider = InputManager.Instance.TouchedCollider;

        if (tappedCollider != null && !tappedCollider.name.Contains("Spawner"))
        {
            // 거품이나 바이러스가 탭된 경우
            HealthObjective objective = tappedCollider.GetComponent<HealthObjective>();
            if (objective != null) objective.Interact();
        }
    }

    private void OnStageEndedGimmik(bool isStageCleared)
    {
        if (isStageCleared)
        {
            Debug.Log("Stage cleared!");
            StageManager.Instance.StageClear(true);
        }
        else
        {
            Debug.Log("Stage failed!");
            StageManager.Instance.StageClear(false);
        }
    }

    // ============ Lifecycle methods ============ //
    public void OnEnable()
    {
        OnStageEnded += OnStageEndedGimmik;
        InputManager.Instance.OnStageTapPerformed += StageGimmikTap;
    }

    public void OnDisable()
    {
        OnStageEnded -= OnStageEndedGimmik;
        InputManager.Instance.OnStageTapPerformed -= StageGimmikTap;
    }

    public void Awake()
    {
        int stageLevel = StageManager.Instance.GetDifficulty();
        
        switch (stageLevel)
        {
            case 1:
                _backgroundGO.GetComponent<Image>().sprite = _backgroundLv1;
                break;
            case 2:
                _backgroundGO.GetComponent<Image>().sprite = _backgroundLv2;
                break;
            case 3:
                _backgroundGO.GetComponent<Image>().sprite = _backgroundLv3;
                break;
            case 4:
                _backgroundGO.GetComponent<Image>().sprite = _backgroundLv4;
                break;
            default:
                Debug.LogError("Invalid stage level: " + stageLevel);
                break;
        }

        _objectiveSpawner.GetComponent<HealthObjectiveSpawner>().Initialize(stageLevel);
    }

    void Start()
    {
        // 스테이지 시작
        OnStageStart();
        SoundManager.Instance.PlayBGM(_stageBGM);
    }

    void Update()
    {
        if (HealthObjective.totalObjectives == 0)
        {
            OnStageClear();
        }
    }
}
