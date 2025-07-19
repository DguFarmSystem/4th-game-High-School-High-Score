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

    private enum StudentState { Idle, Tackling }
    private StudentState _currentState = StudentState.Idle;
    private float _stateTimer = 0f;
    public static float TacklingTime = 0f; // 태클 지속 시간

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

                _leftCollider.enabled = false; // 왼쪽 콜라이더 활성화
                _rightCollider.enabled = false; // 오른쪽 콜라이더 비활성화

                break;

            case StudentState.Tackling:
                _stateTimer = TacklingTime;
                _animator.SetBool("IsTackling", true);

                if (_animator.GetBool("IsLeftSide"))
                {
                    _leftCollider.enabled = true; // 왼쪽 콜라이더 활성화
                    _rightCollider.enabled = false; // 오른쪽 콜라이더 비활성화
                }
                else
                {
                    _leftCollider.enabled = false; // 왼쪽 콜라이더 비활성화
                    _rightCollider.enabled = true; // 오른쪽 콜라이더 활성화
                }

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

    // ============ Lifecycle methods ============ //
    void Awake()
    {
        bool flag = (0 == UnityEngine.Random.Range(0, 2));

        if (flag)
        {
            _studentObject.SetActive(true);
            IsOccupied = true;
            rng = new System.Random(Guid.NewGuid().GetHashCode());
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
