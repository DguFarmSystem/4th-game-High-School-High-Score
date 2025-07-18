using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class DragLineDrawer : MonoBehaviour
{
    [SerializeField] private LineRenderer linePrefab;
    private LineRenderer currentLine;
    private List<Vector3> points = new();

    private InputManager ipManager;
    private bool isDragging = false;

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
        currentLine = Instantiate(linePrefab);
        currentLine.positionCount = 0;
        points.Clear();
        isDragging = true;
        AddPoint(startPos);
    }

    private void UpdateLine(Vector3 newPos)
    {
        if (!isDragging) return;

        // 너무 가까운 지점은 무시
        if (points.Count == 0 || Vector3.Distance(points[^1/*points.Count - 1*/], newPos) > 0.05f)
        {
            AddPoint(newPos);
        }
    }

    private void AddPoint(Vector3 point)
    {
        points.Add(point);
        currentLine.positionCount = points.Count;
        currentLine.SetPositions(points.ToArray());
    }

    private void EndLine()
    {
        isDragging = false;
        Destroy(currentLine.gameObject);
    }

    //============= Lifecycle methods =============//

    void Awake()
    {
        ipManager = FindObjectOfType<InputManager>();
    }

    void OnEnable()
    {
        
        if (ipManager != null)
        {
            Debug.Log("InputManager found, enabling touch input.");
            InputManager._pressAction.started += OnTouch;
            InputManager._positionAction.performed += OnTouch;
            InputManager._pressAction.canceled += OnTouch;
        }
    }

    void OnDisable()
    {
        if (ipManager != null)
        {
            InputManager._pressAction.started -= OnTouch;
            InputManager._positionAction.performed -= OnTouch;
            InputManager._pressAction.canceled -= OnTouch;
        }
    }
}
