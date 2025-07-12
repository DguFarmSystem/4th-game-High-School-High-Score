using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bug : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.8f; // 벌레 이동 속도

    public static int _bugInStage = 0; // 벌레가 스테이지에 있는 개수
    public bool IsAlive => currentState != BugState.Dead; // 벌레가 살아있는지 여부

    private GameObject _windowPrefab;
    private Vector2 _moveDirection;
    private float _stateTimer;
    private Rigidbody2D rb;
    private Animator _animator;
    private Bounds _bugMovingBounds;

    private enum BugState { Idle, Move, Dead }
    private BugState currentState = BugState.Move;

    public void killbug()
    {
        TransitionToState(BugState.Dead);
    }

    private void SetRandomDirection()
    {
        // Z축 회전을 랜덤하게 설정 (0도 ~ 360도)
        float randomAngle = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, 0f, randomAngle);

        // 방향 벡터 계산
        float radians = randomAngle * Mathf.Deg2Rad;
        _moveDirection = new Vector2(-Mathf.Cos(radians), -Mathf.Sin(radians)).normalized; // 방향 벡터를 계산하여 정규화
    }

    private void TransitionToState(BugState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case BugState.Idle:
                _animator.speed = 0f;
                _stateTimer = Random.Range(0.5f, 2f); // 대기 시간 초기화
                break;
            case BugState.Move:
                _animator.speed = 1f;
                _stateTimer = Random.Range(0.5f, 2f); // 이동 시간 초기화
                SetRandomDirection(); // 새로운 방향 설정
                break;
            case BugState.Dead:
                _animator.SetTrigger("isDead"); // 죽은 애니메이션 설정
                transform.rotation = Quaternion.identity;
                _animator.speed = 1f;
                rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY; // 죽어서 떨어지는 연출을 위해 Rigidbody2D 활성화
                _bugInStage--; // 벌레가 죽으면 스테이지 내 벌레 개수 감소
                break;
        }
    }

    private void HandleIdleState()
    {
        _stateTimer -= Time.deltaTime;
        if (_stateTimer <= 0f)
        {
            TransitionToState(BugState.Move); // Idle 상태에서 Move 상태로 전환
        }
    }

    private void HandleMoveState()
    {
        transform.Translate(_moveDirection * moveSpeed * Time.deltaTime, Space.World); // 벌레 이동
        _stateTimer -= Time.deltaTime;

        if (!_bugMovingBounds.Contains(transform.position))
        {
            if (transform.position.x < _bugMovingBounds.min.x || transform.position.x > _bugMovingBounds.max.x)
            {
                float currentZ = transform.rotation.eulerAngles.z;
                float flippedZ = (180f - currentZ) % 360f;
                transform.rotation = Quaternion.Euler(0f, 0f, flippedZ);

                _moveDirection.x = -_moveDirection.x; // X축 경계에 도달하면 방향 반전
                transform.Translate(_moveDirection * moveSpeed * Time.deltaTime, Space.World);
            }

            if (transform.position.y < _bugMovingBounds.min.y || transform.position.y > _bugMovingBounds.max.y)
            {
                float currentZ = transform.rotation.eulerAngles.z;
                float flippedZ = (360f - currentZ) % 360f;
                transform.rotation = Quaternion.Euler(0f, 0f, flippedZ);

                _moveDirection.y = -_moveDirection.y; // Y축 경계에 도달하면 방향 반전
                transform.Translate(_moveDirection * moveSpeed * Time.deltaTime, Space.World);
            }
        }

        if (_stateTimer <= 0f)
        {
            TransitionToState(BugState.Idle); // Move 상태에서 Idle 상태로 전환
        }
    }

    private void HandleDeadState()
    {
        if (transform.position.y < -10f)
        {
            Destroy(gameObject); // 벌레가 화면 아래로 내려가면 제거
        }
    }

    // Lifecyle methods
    void Start()
    {
        if (_windowPrefab == null)
        {
            _windowPrefab = GameObject.Find("Window");
            _bugMovingBounds = _windowPrefab.transform.Find("MovingRange").GetComponent<Collider2D>().bounds;
        }

        rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        SetRandomDirection(); // 초기 방향 설정
        
        _bugInStage++;
    }

    void Update()
    {
        switch (currentState)
        {
            case BugState.Idle:
                HandleIdleState();
                break;
            case BugState.Move:
                HandleMoveState();
                break;
            case BugState.Dead:
                HandleDeadState();
                break;
        }
    }
}
