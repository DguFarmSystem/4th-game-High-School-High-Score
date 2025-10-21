using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelDivider : MonoBehaviour
{
    [Header("분할 설정")]
    public RectTransform targetPanel;
    public int rows = 2;
    public int cols = 4;
    public Camera uiCamera;   // Canvas가 사용하는 Camera (직접 할당)

    private Rect[,] subRects;
    private bool[,] visited;

    public void CalculateSubRects()
    {
        if (targetPanel == null) return;

        visited = new bool[rows, cols];
        subRects = new Rect[rows, cols];

        Vector2 size = targetPanel.rect.size;
        Vector2 pivot = targetPanel.pivot;

        float cellWidth = size.x / cols;
        float cellHeight = size.y / rows;

        float startX = -size.x * pivot.x;
        float startY = -size.y * pivot.y;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                subRects[y, x] = new Rect(
                    startX + x * cellWidth,
                    startY + y * cellHeight,
                    cellWidth,
                    cellHeight
                );
            }
        }
    }

    // 터치 좌표(UI 스크린 좌표) → Panel 로컬 좌표
    public void MarkVisited(Vector2 screenPos)
    {
        if (subRects == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            targetPanel, screenPos, uiCamera, out Vector2 localPos);

        for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
            {
                //if (subRects[y, x].Contains(localPos))
                //{
                //    visited[y, x] = true;
                //    return;
                //}

                if (subRects[y, x].Contains(localPos))
                {
                    if (!visited[y, x])
                    {
                        visited[y, x] = true;
                        Debug.Log($"Cell ({y}, {x}) visited.");
                    }
                    return;
                }
            }
    }

    public bool AllVisited()
    {
        if (visited == null) return false;
        foreach (bool v in visited) if (!v) return false;
        return true;
    }

    //private void OnDrawGizmos()
    //{
    //    if (targetPanel == null)
    //        return;

    //    // 패널 전체 테두리 표시
    //    Vector3[] worldCorners = new Vector3[4];
    //    targetPanel.GetWorldCorners(worldCorners);

    //    Gizmos.color = Color.cyan;
    //    for (int i = 0; i < 4; i++)
    //        Gizmos.DrawLine(worldCorners[i], worldCorners[(i + 1) % 4]);

    //    //n중심점 표시
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawSphere(targetPanel.position, 0.01f);

    //    // 분할 영역 계산 및 표시
    //    CalculateSubRects();

    //    for (int y = 0; y < rows; y++)
    //    {
    //        for (int x = 0; x < cols; x++)
    //        {
    //            // 각 셀의 네 모서리를 로컬 좌표에서 월드 좌표로 변환
    //            Vector3 p1 = targetPanel.TransformPoint(new Vector3(subRects[y, x].xMin, subRects[y, x].yMin, 0));
    //            Vector3 p2 = targetPanel.TransformPoint(new Vector3(subRects[y, x].xMax, subRects[y, x].yMin, 0));
    //            Vector3 p3 = targetPanel.TransformPoint(new Vector3(subRects[y, x].xMax, subRects[y, x].yMax, 0));
    //            Vector3 p4 = targetPanel.TransformPoint(new Vector3(subRects[y, x].xMin, subRects[y, x].yMax, 0));

    //            // 방문 상태에 따라 색상 변경
    //            if (visited != null && visited[y, x])
    //            {
    //                Gizmos.color = new Color(0f, 1f, 0f, 0.25f); // 초록색 반투명
    //                Gizmos.DrawCube((p1 + p3) / 2f, new Vector3(subRects[y, x].width, subRects[y, x].height, 0.001f));
    //            }

    //            Gizmos.color = Color.yellow;
    //            Gizmos.DrawLine(p1, p2);
    //            Gizmos.DrawLine(p2, p3);
    //            Gizmos.DrawLine(p3, p4);
    //            Gizmos.DrawLine(p4, p1);
    //        }
    //    }
    //}
    //private void OnDrawGizmosSelected()
    //{
    //    OnDrawGizmos();
    //}

    void Start()
    {
        CalculateSubRects();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
