using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotePathGizmos : MonoBehaviour
{
    public LineRenderer lineRenderer;

    [Header("Gizmo 설정")]
    public Color pointColor = Color.yellow;
    public float pointSize = 0.06f;

    public Color segmentColor = Color.green;

    private void OnDrawGizmos()
    {
        if (lineRenderer == null)
            return;

        int count = lineRenderer.positionCount;
        if (count == 0)
            return;

        Gizmos.color = pointColor;

        // 각 포인트 표시
        for (int i = 0; i < count; i++)
        {
            Vector3 p = lineRenderer.GetPosition(i);
            Gizmos.DrawSphere(p, pointSize);
        }

        // 선 연결 표시
        Gizmos.color = segmentColor;
        for (int i = 0; i < count - 1; i++)
        {
            Vector3 a = lineRenderer.GetPosition(i);
            Vector3 b = lineRenderer.GetPosition(i + 1);
            Gizmos.DrawLine(a, b);
        }
    }
}
