using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Stage;

public class DragLineDrawer : MonoBehaviour
{
    [SerializeField] private LineRenderer _linePrefab;
    [SerializeField] private Collider2D _playerCollider;
    [SerializeField] private LayerMask _obstacleLayer;

    private LineRenderer _currentLine;
    private List<Vector3> _points = new();

    private FindSeatStage _classRoomStage;
    private InputManager _ipManager;
    private bool _isDragging = false;


    public void OnTouch(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                StartLine(InputManager.TouchWorldPos);
                break;

            case InputActionPhase.Performed:
                UpdateLine(InputManager.TouchWorldPos);
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
        Destroy(_currentLine.gameObject);
    }

    private void AddPoint(Vector3 point)
    {
        _points.Add(point);
        _currentLine.positionCount = _points.Count;
        _currentLine.SetPositions(_points.ToArray());
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
            Debug.Log("InputManager found, enabling touch input.");
            InputManager._pressAction.started += OnTouch;
            InputManager._positionAction.performed += OnTouch;
            InputManager._pressAction.canceled += OnTouch;
        }
    }

    void OnDisable()
    {
        if (_ipManager != null)
        {
            InputManager._pressAction.started -= OnTouch;
            InputManager._positionAction.performed -= OnTouch;
            InputManager._pressAction.canceled -= OnTouch;
        }
    }
    
    void Update()
    {
        if (_classRoomStage && (_classRoomStage.CurrentState == StageState.Clear
                             || _classRoomStage.CurrentState == StageState.Over))
        {
            gameObject.SetActive(false);
        }
    }
}
