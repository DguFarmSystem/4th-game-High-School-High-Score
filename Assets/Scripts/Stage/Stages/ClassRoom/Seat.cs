using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Seat : MonoBehaviour
{
    [SerializeField] private GameObject _studentObject; // 학생 오브젝트
    [SerializeField] private Collider2D _leftCollider; // 왼쪽 콜라이더
    [SerializeField] private Collider2D _rightCollider; // 오른쪽 콜라이더
    public bool IsOccupied { get; private set; } = false; // 좌석이 점유되었는지 여부

    private static bool _isAllSeatOccupied = false; // 모든 좌석이 점유되었는지 여부
    // 모든 좌석이 점유된 경우 목적지가 없어 스테이지 자체가 오류 발생
    // 그러나 그 확률이 극히 낮아 따로 장치는 하지 않았음

    private enum StudentState { Idle, Tackling }
    private StudentState _currentState = StudentState.Idle;
    private float _stateTimer = 0f;
    private float _tacklingTime = 0f; // 태클 지속 시간

    private Animator _animator;
    
    private System.Random rng;

    private void TransitionToState(StudentState newState)
    {
        _currentState = newState;

        switch (newState)
        {
            case StudentState.Idle:
                _stateTimer = (float)(rng.NextDouble() * (2f - 0.5f) + 0.5f); // 대기 시간 초기화
                _animator.SetBool("IsTackling", false);

                break;

            case StudentState.Tackling:
                _stateTimer = _tacklingTime;
                _animator.SetBool("IsTackling", true);

                break;
        }
    }

    private void HandleIdleState()
    {
        _stateTimer -= Time.deltaTime;
        if (_stateTimer <= 0f)
        {
            TransitionToState(StudentState.Tackling); // Idle 상태에서 Tackling 상태로 전환
        }
    }

    private void HandleTacklingState()
    {
        _stateTimer -= Time.deltaTime;
        if (_stateTimer <= 0f)
        {
            TransitionToState(StudentState.Idle); // Idle 상태에서 Tackling 상태로 전환
        }
    }

    // =========== Animation Event =========== //
    public void EnableLeftCollider()
    {
        _leftCollider.enabled = true;
    }

    public void EnableRightCollider()
    {
        _rightCollider.enabled = true;
    }

    public void DisableColliders()
    {
        _leftCollider.enabled = false;
        _rightCollider.enabled = false;
    }

    // ============ Lifecycle methods ============ //
    void Awake()
    {
        ClassRoomStage stage = FindObjectOfType<ClassRoomStage>();

        if (stage)
        {
            int stageLevel = stage.stageLevel;
            bool flag = (UnityEngine.Random.Range(0, 2) == 0); // 무작위로 true 또는 false 설정

            switch (stageLevel)
            {
                case 1:

                    break;

                case 2:
                    if (flag)
                    {
                        _studentObject.SetActive(true);
                        IsOccupied = true;
                        _tacklingTime = 1.0f;
                    }

                    break;

                case 3:
                    if (flag)
                    {
                        _studentObject.SetActive(true);
                        IsOccupied = true;
                        _tacklingTime = 2.0f;
                    }

                    break;

                default:
                    Debug.LogWarning("Unknown stage level: " + stageLevel);

                    break;
            }

            rng = new System.Random(Guid.NewGuid().GetHashCode());
        }

        // 모든 좌석이 점유된 경우의 장치, 현재 작동X
        if (_isAllSeatOccupied)
        {
            _studentObject.SetActive(false);
            IsOccupied = false;
            _tacklingTime = 0f;
        } 
    }
    void Start()
    {
        if (_studentObject.activeSelf)
        {
            _animator = _studentObject.GetComponent<Animator>();

            if (transform.parent != null && transform.parent.CompareTag("LeftSide"))
            {
                _animator.SetBool("IsLeftSide", true);
            }
            else if (transform.parent != null && transform.parent.CompareTag("RightSide"))
            {
                _animator.SetBool("IsLeftSide", false);
            }
            else
            {
                Debug.LogError("Parent object does not have a valid tag for left or right side.");
            }

            _stateTimer = (float)(rng.NextDouble() * (2f - 0.5f) + 0.5f); // 초기 대기 시간 설정
        }
    }
    
    void Update()
    {
        if (_studentObject.activeSelf)
        {
            switch (_currentState)
            {
                case StudentState.Idle:
                    HandleIdleState();

                    break;

                case StudentState.Tackling:
                    HandleTacklingState();

                    break;
            }
        }
    }
}
