using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

using Stage;

public class DragLineDrawer : MonoBehaviour
{
    [SerializeField] private LineRenderer _linePrefab;
    [SerializeField] private Collider2D _playerCollider;
    [SerializeField] private LayerMask _obstacleLayer;

    [SerializeField] private AudioClip _drawLineSfx;

    private LineRenderer _currentLine;
    private List<Vector3> _points = new();

    private FindSeatStage _classRoomStage;
    private InputManager _ipManager;
    private bool _isDragging = false;
    private float _idleTime = 0f;
    private bool _isCheckingIdle = false;


    public void OnTouch(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                StartLine(InputManager.Instance.TouchWorldPos);
                break;

            case InputActionPhase.Performed:
                UpdateLine(InputManager.Instance.TouchWorldPos);
                break;

            case InputActionPhase.Canceled:
                EndLine();
                break;
        }
    }

    private void StartLine(Vector3 startPos)
    {
        if (!_playerCollider.OverlapPoint(startPos)) return;

        _currentLine = Instantiate(_linePrefab);
        _currentLine.positionCount = 0;
        _points.Clear();
        _isDragging = true;
        AddPoint(startPos);
        //SoundManager.Instance.PlayStoppableSFX(_drawLineSfx);
    }

    private void UpdateLine(Vector3 newPos)
    {
        if (!_isDragging) return;

        if (_classRoomStage.CurrentState != StageState.Playing)
        {
            return;
        }

        Collider2D hit = Physics2D.OverlapPoint(newPos, _obstacleLayer);

        if (hit != null)
        {
            if (hit == _classRoomStage.Goal.GetComponent<Collider2D>())
            {
                // 목표 좌석에 닿았을 때 라인 드로잉 중지
                _classRoomStage.stageClearFlag = true;
                return;
            }

            // 장애물에 닿았을 때 라인 드로잉 중지
            EndLine();
            return;
        }
        // 너무 가까운 지점은 무시
        if (_points.Count == 0 || Vector3.Distance(_points[^1/*points.Count - 1*/], newPos) > 0.05f)
        {
            AddPoint(newPos);
        }
    }

    private void EndLine()
    {
        if (!_isDragging) return;

        _isDragging = false;
        //SoundManager.Instance.StopStoppableSFX();
        Destroy(_currentLine.gameObject);
    }

    private void AddPoint(Vector3 point)
    {
        _idleTime = 0f;
        if (!_isCheckingIdle)
            StartCoroutine(CheckIdleTime());
        _points.Add(point);
        _currentLine.positionCount = _points.Count;
        _currentLine.SetPositions(_points.ToArray());
    }

    private IEnumerator CheckIdleTime()
    {
        _isCheckingIdle = true;
        while (_isDragging) 
        {
            if (_idleTime > 0.2f)
            {
                //SoundManager.Instance.StopStoppableSFX();
                break;
            }
            _idleTime += Time.deltaTime;
            yield return null;
        }
        _isCheckingIdle = false;
    }

    //============= Lifecycle methods =============//

    void Awake()
    {
        _ipManager = FindObjectOfType<InputManager>();
        _classRoomStage = FindObjectOfType<FindSeatStage>();
    }

    void Start()
    {

        if (_ipManager != null)
        {
            InputManager.Instance._pressAction.started += OnTouch;
            InputManager.Instance._positionAction.performed += OnTouch;
            InputManager.Instance._pressAction.canceled += OnTouch;
        }
    }

    void OnDisable()
    {
        if (_ipManager != null)
        {
            InputManager.Instance._pressAction.started -= OnTouch;
            InputManager.Instance._positionAction.performed -= OnTouch;
            InputManager.Instance._pressAction.canceled -= OnTouch;
        }
    }
    
    void Update()
    {
        if (_classRoomStage && (_classRoomStage.CurrentState == StageState.Clear
                             || _classRoomStage.CurrentState == StageState.Over))
        {
            //SoundManager.Instance.StopStoppableSFX();
            gameObject.SetActive(false);
        }
    }
}
