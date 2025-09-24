using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using Stage;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;

public class WindowClosingStage : StageNormal
{
    private enum StageClearConditionType
    {
        CloseWindow,   // 창문 닫기
        CloseBlind,    // 블라인드 내리기
        KillBugs,      // 벌레 죽이기
    }

    private class StageClearCondition
    {
        public StageClearConditionType conditionType { get; private set; }

        public StageClearCondition(StageClearConditionType type)
        {
            conditionType = type;
        }
    }

    private Stack<StageClearCondition> _stageClearConditions = new Stack<StageClearCondition>();
    private int _bugSpawnCount = 0;
    private int _bugInStage = 0; // 벌레가 스테이지에 있는 개수

    private Rigidbody2D _windowRd;
    private Bounds _windowBounds;
    private Bounds _windowMovingRange;
    private Rigidbody2D _movingBlindRd;
    private Bounds _fixedBlindBounds;
    private Bounds _movingBlindBounds;

    [SerializeField] public int stageLevel;
    [SerializeField] private GameObject _windowPrefab;
    [SerializeField] private GameObject _bugPrefab;
    [SerializeField] private GameObject _windowBrokenPrefab;
    [SerializeField] private GameObject _blindPrefab;
    //TEST CODE
    [SerializeField] private GameObject _greenSphere;
    [SerializeField] private GameObject _redSphere;


    private void StageGimmikTap()
    {
        switch (_stageClearConditions.Peek().conditionType)
        {
            case StageClearConditionType.CloseWindow:

                break;

            case StageClearConditionType.CloseBlind:

                break;

            case StageClearConditionType.KillBugs:
                Collider2D touchedCollider = InputManager.TouchedCollider;

                if (touchedCollider != null)
                {
                    Bug bug = touchedCollider.gameObject.GetComponent<Bug>();
                    
                    if (bug != null && bug.IsAlive)
                    {
                        // 벌레를 죽임
                        bug.killbug();
                        _bugInStage--; // 벌레가 죽으면 스테이지 내 벌레 개수 감소
                        Debug.Log("Bugs in stage: " + _bugInStage);
                        Collider2D[] hits = Physics2D.OverlapPointAll(InputManager.TouchWorldPos);
                        Collider2D windowCol;

                        windowCol = hits
                            .Where(hit => hit.GetComponent<SpriteRenderer>() != null && !hit.CompareTag("Bug")
                            && !hit.CompareTag("Blind") && !hit.CompareTag("MovingRange")) // SpriteRenderer가 null이 아닌 경우만 선택
                            .OrderBy(hit => hit.GetComponent<SpriteRenderer>().sortingOrder) // sortingOrder 기준으로 정렬
                            .LastOrDefault();

                        GameObject broken = Instantiate(_windowBrokenPrefab, InputManager.TouchWorldPos, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

                        if (broken != null)
                        {
                            broken.transform.SetParent(windowCol.transform, true);
                        }

                        if (_bugInStage <= 0)
                        {
                            _stageClearConditions.Pop(); // 벌레가 모두 죽으면 조건을 제거
                        }
                    }
                }
                break;

            default:
                Debug.LogWarning("Unknown stage clear condition type.");
                break;
        }
    }
    
    private void OnStageEndedGimmik(bool isStageCleared)
    {
        InputManager.OnStageTapPerformed -= StageGimmikTap; // 이벤트 구독 해제

        if (isStageCleared)
        {
            //TEST CODE
            Debug.Log("Stage cleared!");
            StageManager.Instance.StageClear(true);
        }
        else
        {
            //TEST CODE
            Debug.Log("Stage failed!");
            StageManager.Instance.StageClear(false);
        }
    }

    private void SpawnMultiPrefabs(GameObject prefab, int SpawnCount, Bounds GeneratingBounds)
    {
        List<Bounds> GeneratedBounds = new List<Bounds>();

        for (int j = 0; j < SpawnCount; j++)
        {
            Vector3 randomPos;
            bool isValidPos;

            do // 스폰 유효성 확인
            {
                // 범위 내에서 랜덤 위치 선정
                randomPos = new Vector3(
                    Random.Range(GeneratingBounds.min.x + 1f, GeneratingBounds.max.x - 1f),
                    Random.Range(GeneratingBounds.min.y + 1f, GeneratingBounds.max.y - 1f),
                    0f
                );

                isValidPos = true;

                // 이미 생성된 벌레와 겹치는 위치인지 확인
                foreach (Bounds bound in GeneratedBounds)
                {
                    if (bound.Contains(randomPos))
                    {
                        isValidPos = false; // 위치가 유효하지 않음
                        break; // 더 이상 확인할 필요 없음
                    }
                }
            } while (!isValidPos); // 위치가 유효하지 않으면 다시 반복

            GameObject spawnedPrefab = Instantiate(prefab, randomPos, Quaternion.identity);
            GeneratedBounds.Add(new Bounds(randomPos, spawnedPrefab.GetComponent<Collider2D>().bounds.size * 1.5f));
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
        Debug.Log("Stage Clear!");
    }


    //==================== Lifecycle methods ====================//
    public void Awake()
    {

    }

    public void OnEnable()
    {

        InputManager.OnStageTapPerformed += StageGimmikTap;
        OnStageEnded += OnStageEndedGimmik;
    }

    public void OnDisable()
    {
        InputManager.OnStageTapPerformed -= StageGimmikTap;
        OnStageEnded -= OnStageEndedGimmik;
    }

    void Start()
    {
        for (int i = 0; i < stageLevel; i++)
        {
            switch (i)
            {
                case 0: // CloseWindow
                    _windowRd = _windowPrefab.GetComponentInChildren<Rigidbody2D>();
                    _windowMovingRange = _windowPrefab.transform.Find("MovingRange").GetComponent<Collider2D>().bounds;
                    
                    if (i == stageLevel - 1)
                    {
                        _stageClearConditions.Push(new StageClearCondition(StageClearConditionType.CloseWindow));
                    }

                    break;
                case 1: // CloseBlind
                    _blindPrefab.SetActive(true);

                    _movingBlindRd = _blindPrefab.GetComponentInChildren<Rigidbody2D>();
                    _fixedBlindBounds = _blindPrefab.transform.Find("Default_Blind").GetComponent<Collider2D>().bounds;

                    if (i == stageLevel - 1)
                    {
                        _stageClearConditions.Push(new StageClearCondition(StageClearConditionType.CloseBlind));
                        _stageClearConditions.Push(new StageClearCondition(StageClearConditionType.CloseWindow));
                    }

                    break;
                case 2: // KillBugs
                    Bounds bugGeneratingBounds = _windowPrefab.transform.Find("MovingRange").GetComponent<Collider2D>().bounds;
                    _bugSpawnCount = Random.Range(1, 4); // 1~3마리의 벌레 생성

                    SpawnMultiPrefabs(_bugPrefab, _bugSpawnCount, bugGeneratingBounds);
                    _bugInStage = _bugSpawnCount;

                    if (i == stageLevel - 1)
                    {
                        _stageClearConditions.Push(new StageClearCondition(StageClearConditionType.CloseBlind));
                        _stageClearConditions.Push(new StageClearCondition(StageClearConditionType.CloseWindow));
                        _stageClearConditions.Push(new StageClearCondition(StageClearConditionType.KillBugs));
                    }

                    break;
                default:
                    Debug.LogWarning("Unknown stage level: " + i);
                    break;
            }
        }

        // 스테이지 시작
        OnStageStart();
    }

    void Update()
    {
        if (_stageClearConditions.Count == 0 && CurrentStageState == StageState.Playing)
        {
            OnStageClear(); // 모든 조건이 완료되면 스테이지 클리어 처리
        }
    }
    

    void FixedUpdate()
    {
        if (CurrentStageState == StageState.Playing &&
            InputManager.IsPressing &&
            _stageClearConditions.Count > 0)
        {
            Collider2D col = InputManager.PressedCollider;

            if (col != null)
            {
                GameObject obj = col.gameObject;

                switch (_stageClearConditions.Peek().conditionType)
                {
                    case StageClearConditionType.CloseWindow:
                        _windowBounds = _windowPrefab.transform.Find("Moving_Window").GetComponent<Collider2D>().bounds;

                        if (obj.CompareTag("Window"))
                        {
                            Vector2 moveVec = Vector2.right * InputManager.Delta.x;

                            float leftRange = _windowMovingRange.min.x - _windowBounds.min.x;
                            float rightRange = _windowMovingRange.max.x - _windowBounds.max.x;

                            moveVec.x = Mathf.Clamp(moveVec.x, leftRange, rightRange);

                            Vector2 newPosition = _windowRd.position + moveVec;

                            _windowRd.MovePosition(newPosition);

                            // 창문이 닫히면 조건 처리
                            if (_windowBounds.max.x >= _windowMovingRange.max.x)
                            {
                                _stageClearConditions.Pop(); // 조건 완료 처리
                            }
                        }

                        break;

                    case StageClearConditionType.CloseBlind:
                        _movingBlindBounds = _blindPrefab.transform.Find("Moving_Blind").GetComponent<Collider2D>().bounds;

                        if (obj.CompareTag("Handle"))
                        {
                            Vector2 moveVec = Vector2.up * InputManager.Delta.y;

                            float upRange = _fixedBlindBounds.max.y - _movingBlindBounds.max.y;
                            float downRange = _fixedBlindBounds.min.y - _movingBlindBounds.max.y;

                            moveVec.y = Mathf.Clamp(moveVec.y, downRange, upRange);

                            Vector2 newPosition = _movingBlindRd.position + moveVec;

                            _movingBlindRd.MovePosition(newPosition);

                            // 블라인드가 닫히면 조건 처리
                            if (_movingBlindBounds.max.y <= _fixedBlindBounds.min.y)
                            {
                                _stageClearConditions.Pop(); // 조건 완료 처리
                            }
                        }

                        break;

                    case StageClearConditionType.KillBugs:

                        break;

                    default:
                        Debug.LogWarning("Unknown stage clear condition type.");

                        break;
                }
            }
        }
    }
}
