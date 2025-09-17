using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Student : MonoBehaviour
{
    [SerializeField] private Collider2D _leftCollider; // 왼쪽 콜라이더
    [SerializeField] private Collider2D _rightCollider; // 오른쪽 콜라이더

    [SerializeField] private Collider2D _leftHandCollider; // 왼쪽 손 콜라이더
    [SerializeField] private Collider2D _rightHandCollider; // 오른쪽 손 콜라이더

    private Animator _animator;

    private enum StudentState { Idle, Tackling }
    private StudentState _currentState = StudentState.Idle;
    private float _stateTimer = 0f;
    private float _tacklingTime = 2f; // 태클 지속 시간

    private System.Random rng;

    private void TransitionToState(StudentState newState)
    {
        _currentState = newState;

        switch (newState)
        {
            case StudentState.Idle:
                _stateTimer = (float)(rng.NextDouble() * (2f - 1f) + 1f); // 대기 시간 초기화
                _animator.SetBool("IsTackling", false);
                _animator.SetBool("IsHandTackling", false);

                break;

            case StudentState.Tackling:
                _stateTimer = _tacklingTime;
                int randnum = rng.Next(0, 2); // 0 또는 1 생성
                if (randnum == 0)
                {
                    _animator.SetBool("IsTackling", true);
                }
                else
                {
                    _animator.SetBool("IsHandTackling", true);
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

    // =========== Animation Event =========== //
    public void EnableLeftCollider()
    {
        _leftCollider.enabled = true;
    }

    public void EnableRightCollider()
    {
        _rightCollider.enabled = true;
    }

    public void EnableLeftHandCollider()
    {
        _leftHandCollider.enabled = true;
    }

    public void EnableRightHandCollider()
    {
        _rightHandCollider.enabled = true;
    }

    public void DisableColliders()
    {
        _leftCollider.enabled = false;
        _rightCollider.enabled = false;
        _leftHandCollider.enabled = false;
        _rightHandCollider.enabled = false;
    }

    // ========== LifeCyle Methods ========== //
    void Start()
    {
        _animator = GetComponent<Animator>();

        rng = new System.Random(Guid.NewGuid().GetHashCode());

        if (transform.parent != null && transform.parent.parent.CompareTag("LeftSide"))
        {
            _animator.SetBool("IsLeftSide", true);
        }
        else if (transform.parent != null && transform.parent.parent.CompareTag("RightSide"))
        {
            _animator.SetBool("IsLeftSide", false);
        }
        else
        {
            Debug.LogError("Parent object does not have a valid tag for left or right side.");
        }

        _stateTimer = (float)(rng.NextDouble() * (2f - 0.5f) + 0.5f); // 초기 대기 시간 설정
    }
    void Update()
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
