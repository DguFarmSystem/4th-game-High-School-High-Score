using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teacher : MonoBehaviour
{
    [SerializeField] private GameObject _sparkle;
    private Animator _animator;

    private enum StudentState { Idle, Turning }
    private StudentState _currentState = StudentState.Idle;
    private float _stateTimer = 0f;
    private float _turningTime = 2f; // 뒤돌아 보고 있는 시간

    public bool IsTurning => _currentState == StudentState.Turning;

    private void TransitionToState(StudentState newState)
    {
        _currentState = newState;

        switch (newState)
        {
            case StudentState.Idle:
                _stateTimer = Random.Range(3f, 6f); // 대기 시간 초기화
                _animator.SetBool("IsTurning", false);

                break;

            case StudentState.Turning:
                _stateTimer = _turningTime;
                _animator.SetBool("IsTurning", true);
                _animator.speed = 1f; // 애니메이션 속도 복원

                break;
        }
    }

    private void HandleIdleState()
    {
        _stateTimer -= Time.deltaTime;

        if (_stateTimer <= 1f)
        {
            _animator.speed = 0f;

            if (_stateTimer <= 0f)
            {
                TransitionToState(StudentState.Turning); // Idle 상태에서 Turning 상태로 전환
            }
        }

    }

    private void HandleTurningState()
    {
        _stateTimer -= Time.deltaTime;
        if (_stateTimer <= 0f)
        {
            TransitionToState(StudentState.Idle); // Turning 상태에서 Idle 상태로 전환
        }
    }

    // =========== Animation Event =========== //
    private void EnableSparkle()
    {
        _sparkle.SetActive(true);
    }

    private void DisableSparkle()
    {
        if (_sparkle.activeSelf)
        {
            AnimatorStateInfo stateInfo = _sparkle.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0); // 0은 기본 레이어
            if (stateInfo.normalizedTime >= 1f) _sparkle.SetActive(false);
        }
        
    }

    // ========== LifeCyle Methods ========== //
    void Start()
    {
        _animator = GetComponent<Animator>();

        _stateTimer = Random.Range(3f, 6f);
    }

    void Update()
    {
        switch (_currentState)
        {
            case StudentState.Idle:
                HandleIdleState();

                break;

            case StudentState.Turning:
                HandleTurningState();

                break;
        }
    }
}
