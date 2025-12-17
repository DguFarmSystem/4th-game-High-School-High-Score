using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teacher : MonoBehaviour
{
    [SerializeField] private ParticleSystem _sparkle;
    [SerializeField] private AudioClip _turningSfx;

    private Animator _animator;

    private enum State { Idle, Turning }
    private State _currentState = State.Idle;
    private float _stateTimer = 0f;
    private float _turningTime = 2f; // 뒤돌아 보고 있는 시간

    public bool IsTurning => _currentState == State.Turning;

    private void TransitionToState(State newState)
    {
        _currentState = newState;

        switch (newState)
        {
            case State.Idle:
                _stateTimer = Random.Range(3f, 6f); // 대기 시간 초기화
                _animator.SetBool("IsTurning", false);

                break;

            case State.Turning:
                _stateTimer = _turningTime;
                _animator.SetBool("IsTurning", true);
                _animator.speed = 1f; // 애니메이션 속도 복원
                SoundManager.Instance.PlaySFX(_turningSfx);

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
                TransitionToState(State.Turning); // Idle 상태에서 Turning 상태로 전환
            }
        }

    }

    private void HandleTurningState()
    {
        _stateTimer -= Time.deltaTime;
        if (_stateTimer <= 0f)
        {
            TransitionToState(State.Idle); // Turning 상태에서 Idle 상태로 전환
        }
    }

    // =========== Animation Event =========== //
    private IEnumerator EnableSparkle()
    {
        yield return new WaitForSeconds(0.2f); // 약간의 딜레이 후에 스파클 효과 시작
        _sparkle.Play();
    }

    private void DisableSparkle()
    {
        _sparkle.Stop();
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
            case State.Idle:
                HandleIdleState();

                break;

            case State.Turning:
                HandleTurningState();

                break;
        }
    }
}
