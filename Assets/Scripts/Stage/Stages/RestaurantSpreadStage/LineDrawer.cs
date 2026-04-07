using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LineDrawer : MonoBehaviour
{
    [SerializeField] private LineRenderer linePrefab;
    [SerializeField] private PanelDivider sectionCheck;
    [SerializeField] private Camera mainCam;
    [SerializeField] private RectTransform sauceImage;
    [SerializeField] private AudioSource EffectAudio;
    [SerializeField] private AudioClip EffectSound;

    private LineRenderer currentLine;
    private List<Vector3> points = new();

    private RestaurantSpreadStage restaurantStage;
    private InputManager inputManager;
    private bool isDragging = false;

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

    public void StartLine(Vector3 startPos)
    {
        currentLine = Instantiate(linePrefab);
        currentLine.positionCount = 0;
        points.Clear();
        isDragging = true;
        //Debug.Log(isDragging);
        AddPoint(startPos);
        //오디오 재생
        EffectAudio.Play();
    }

    public void UpdateLine(Vector3 newPos)
    {
        if (!isDragging) return;
        //Debug.Log(isDragging);

        // 너무 가까운 지점은 무시
        if (points.Count == 0 || Vector3.Distance(points[^1/*points.Count - 1*/], newPos) > 0.05f)
        {
            AddPoint(newPos);
        }
        Vector2 newPostoUIPos = RectTransformUtility.WorldToScreenPoint(mainCam, newPos);
        TranslatePosition(newPostoUIPos);
        sectionCheck.MarkVisited(newPostoUIPos);
    }

    public void EndLine()
    {
        isDragging = false;
        currentLine = null;
        points.Clear();
    }

    //LineRenderer에 점 위치 추가
    private void AddPoint(Vector3 point)
    {
        points.Add(point);
        currentLine.positionCount = points.Count;
        currentLine.SetPositions(points.ToArray());
    }

    private void TranslatePosition(Vector2 screenPos)
    {
        if (sauceImage == null) return;

        RectTransform canvasRect = sauceImage.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, mainCam, out Vector2 localPos))
        {
            sauceImage.anchoredPosition = localPos;
        }
    }


    void Awake()
    {
        restaurantStage = FindObjectOfType<RestaurantSpreadStage>();
        inputManager = FindObjectOfType<InputManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        EffectAudio.clip = EffectSound;
    }

    private void OnEnable()
    {
        //fetch event
        if (inputManager != null)
        {
            InputManager.Instance._pressAction.started += OnTouch;
            InputManager.Instance._positionAction.performed += OnTouch;
            InputManager.Instance._pressAction.canceled += OnTouch;
        }
    }

    private void OnDisable()
    {
        //detach event
        if (inputManager != null)
        {
            InputManager.Instance._pressAction.started -= OnTouch;
            InputManager.Instance._positionAction.performed -= OnTouch;
            InputManager.Instance._pressAction.canceled -= OnTouch;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
