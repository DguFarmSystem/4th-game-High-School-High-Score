using System.Collections;
using System.Collections.Generic;
using Stage;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PutAwayStage : StageNormal
{
    [Header("Objects by Level")]
    [SerializeField] private GameObject bg_LowLevel;
    [SerializeField] private GameObject bg_HighLevel;
    [SerializeField] private GameObject basket_LowLevel;
    [SerializeField] private GameObject basket_HighLevel;

    [Header("Real Ball Prefabs")]
    [SerializeField] private GameObject basketball_Prefab;
    [SerializeField] private GameObject blueBall_Prefab;
    [SerializeField] private GameObject greenBall_Prefab;
    [SerializeField] private GameObject redBall_Prefab;
    [SerializeField] private GameObject waterMelonBall1_Prefab;
    [SerializeField] private GameObject waterMelonBall2_Prefab;
    [SerializeField] private GameObject soccerBall_Prefab;

    [Header("Fake Ball Prefabs")]
    [SerializeField] private GameObject globe_Prefab;
    [SerializeField] private GameObject waterMelon_Prefab;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip _stageBGM;
                                                
    public StageState CurrentState => CurrentStageState;

    private GameObject _currentBall = null;
    private List<GameObject> _allBalls;

    private GameObject _activeBasket;

    public void OnTouch(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                StartTouch();
                break;

            case InputActionPhase.Performed:
                UpdateTouch(InputManager.Instance.TouchWorldPos);
                break;

            case InputActionPhase.Canceled:
                EndTouch();
                break;
        }
    }

    private void StartTouch()
    {
        if (_currentBall != basket_LowLevel && _currentBall != basket_HighLevel)
        {
            _currentBall = InputManager.Instance.PressedCollider?.gameObject;
        }
    }

    private void UpdateTouch(Vector3 newPos)
    {
        if (_currentBall != null)
        {
            _currentBall.transform.position = newPos;
        }
    }

    private void EndTouch()
    {
        _currentBall = null;
    }

    private void OnStageEndedGimmik(bool isStageCleared)
    {
        SoundManager.Instance.StopBGM();
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

    void SpawnBalls(int count)
    {
        if (_allBalls.Count == 0) return;

        Camera cam = Camera.main;
        float dist = Mathf.Abs(cam.transform.position.z);

        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, dist));
        Vector3 topright = cam.ViewportToWorldPoint(new Vector3(1, 1, dist));

        foreach (var ball in _allBalls)
        {
            Vector3 spawnPos;
            while(true)
            {
                spawnPos = new Vector3(
                    Random.Range(bottomLeft.x+1.5f, topright.x-1.5f),
                    Random.Range(bottomLeft.y+1f, topright.y-1f),
                    0
                );

                if (_activeBasket.GetComponent<Collider2D>().OverlapPoint(spawnPos))
                    continue;
                else
                    break;
            }

            Instantiate(ball, spawnPos, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
        }
    }

    // ============ Lifecycle methods ============ //
    public void OnEnable()
    {
        OnStageEnded += OnStageEndedGimmik;
        InputManager.Instance._pressAction.started += OnTouch;
        InputManager.Instance._positionAction.performed += OnTouch;
        InputManager.Instance._pressAction.canceled += OnTouch;
    }

    public void OnDisable()
    {
        OnStageEnded -= OnStageEndedGimmik;
        InputManager.Instance._pressAction.started -= OnTouch;
        InputManager.Instance._positionAction.performed -= OnTouch;
        InputManager.Instance._pressAction.canceled -= OnTouch;
    }

    void Awake()
    {
        int level = StageManager.Instance.GetDifficulty();
        _allBalls = level switch
        {
            1 => new List<GameObject> { basketball_Prefab, basketball_Prefab, basketball_Prefab }, //3
            2 => new List<GameObject> { soccerBall_Prefab, soccerBall_Prefab, waterMelon_Prefab, globe_Prefab }, //4
            3 => new List<GameObject> { redBall_Prefab, blueBall_Prefab, waterMelonBall1_Prefab }, //3
            4 => new List<GameObject> { redBall_Prefab, greenBall_Prefab, blueBall_Prefab,
                                        waterMelonBall1_Prefab,waterMelonBall2_Prefab, waterMelon_Prefab }, //6
            _ => new List<GameObject>()
        };

        switch (level)
        {
            case 1:
            case 2:
                bg_LowLevel.SetActive(true);
                basket_LowLevel.SetActive(true);
                _activeBasket = basket_LowLevel;
                break;

            case 3:
            case 4:
                bg_HighLevel.SetActive(true);
                basket_HighLevel.SetActive(true);
                _activeBasket = basket_HighLevel;
                break;

            default:
                Debug.LogError("Invalid stage level: " + level);
                break;
        }
        SpawnBalls(_allBalls.Count);
    }

    void Start()
    {
        // 스테이지 시작
        OnStageStart();
        //SoundManager.Instance.PlayBGM(_stageBGM);
    }

    
    void Update()
    {   
        /*
        if (CurrentStageState == StageState.Playing)
        {
            OnStageClear(); // 모든 조건이 완료되면 스테이지 클리어 처리
        }
        */
    }
}
