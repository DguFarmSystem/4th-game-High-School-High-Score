using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 컴포넌트 접근을 위해 필수
using UnityEngine.EventSystems; // UI 레이캐스트를 위해 필수

public class HeartLineDrawing : MonoBehaviour
{
    public enum ColorType { None, Blue, Red }

    [Header("Components")]
    private LineRenderer lineRenderer;

    [Header("UI Raycast Settings")]
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private EventSystem eventSystem;

    [Header("State")]
    private bool isDrawing = false;
    private bool isGameClear = false;
    private int pointCount = 0;
    private ColorType currentLineColor = ColorType.None;

    // 현재까지 연결에 성공한 총 선의 개수 (7개가 되면 클리어)
    private int totalConnectedLines = 0;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (graphicRaycaster == null) graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
        if (eventSystem == null) eventSystem = FindObjectOfType<EventSystem>();

        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    void Update()
    {
        if (isGameClear) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    TryStartDrawingUI(touch.position);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (isDrawing)
                    {
                        DrawLineUI(touch.position);
                        CheckEndZoneUI(touch.position);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (isDrawing)
                    {
                        ResetLine();
                    }
                    break;
            }
        }
    }

    void TryStartDrawingUI(Vector2 touchPos)
    {
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = touchPos;

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        if (results.Count > 0)
        {
            GameObject hitUI = results[0].gameObject;
            Vector3 startPos = GetCanvasWorldPosition(touchPos);

            // 이름 제한 없이 태그만 맞으면 언제든 그리기 시작 가능
            if (hitUI.CompareTag("BlueStart"))
            {
                currentLineColor = ColorType.Blue;
                SetLineRendererColor(Color.blue);
                StartLine(startPos);
            }
            else if (hitUI.CompareTag("RedStart"))
            {
                currentLineColor = ColorType.Red;
                SetLineRendererColor(Color.red);
                StartLine(startPos);
            }
        }
    }

    void StartLine(Vector3 startPos)
    {
        isDrawing = true;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, startPos);
        pointCount = 1;
        Debug.Log($"<color=cyan>[Line Start]</color> {currentLineColor} 선 그리기 시작");
    }

    void SetLineRendererColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    void DrawLineUI(Vector2 touchPos)
    {
        Vector3 currentPoint = GetCanvasWorldPosition(touchPos);

        if (pointCount == 1)
        {
            lineRenderer.positionCount = 2;
            pointCount = 2;
        }

        lineRenderer.SetPosition(pointCount - 1, currentPoint);
    }

    void CheckEndZoneUI(Vector2 touchPos)
    {
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = touchPos;

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        if (results.Count > 0)
        {
            GameObject hitUI = results[0].gameObject;

            // 파란 선 -> 파란 도착지
            if (currentLineColor == ColorType.Blue && hitUI.CompareTag("BlueEnd"))
            {
                CompleteDrawing(hitUI.transform.position);
            }
            // 빨간 선 -> 빨간 도착지
            else if (currentLineColor == ColorType.Red && hitUI.CompareTag("RedEnd"))
            {
                CompleteDrawing(hitUI.transform.position);
            }
        }
    }

    void CompleteDrawing(Vector3 targetPosition)
    {
        isDrawing = false;

        // 최종 목적지 좌표 고정
        lineRenderer.SetPosition(pointCount - 1, new Vector3(targetPosition.x, targetPosition.y, targetPosition.z));

        // 총 성공 연결 개수 증가
        totalConnectedLines++;
        Debug.Log($"<color=green>★ 연결 성공! ★ | 현재 누적 연결 선: {totalConnectedLines}/7</color>");

        // 현재 그린 선을 화면에 복제(박제)하여 남기기
        GameObject finishedLine = Instantiate(this.gameObject, transform.parent);
        Destroy(finishedLine.GetComponent<HeartLineDrawing>());

        // 본체 리셋 후 다음 터치 대기
        lineRenderer.positionCount = 0;
        currentLineColor = ColorType.None;

        // 최종 7개 클리어 조건 검사
        if (totalConnectedLines >= 7)
        {
            isGameClear = true;
            Debug.Log("<color=magenta>♥♥♥♥ 모든 선(7개) 연결 완료! 게임 클리어! ♥♥♥?</color>");
        }
    }

    void ResetLine()
    {
        isDrawing = false;
        pointCount = 0;
        lineRenderer.positionCount = 0;
        currentLineColor = ColorType.None;
        Debug.Log("<color=gray>[Line Reset] 연결 실패로 선 초기화.</color>");
    }

    private Vector3 GetCanvasWorldPosition(Vector2 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
        worldPos.z = transform.position.z;
        return worldPos;
    }
}
