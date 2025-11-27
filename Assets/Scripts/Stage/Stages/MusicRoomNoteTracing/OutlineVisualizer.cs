using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineVisualizer : MonoBehaviour
{
    public OutlineExtractor extractor;    // Assign in Inspector
    public bool loop = true;              // Close the shape
    public float lineWidth = 0.05f;

    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();

        if (extractor == null)
        {
            Debug.LogError("OutlineVisualizer: No SpriteOutlineExtractor assigned.");
            return;
        }

        DrawOutline();
    }

    public void DrawOutline()
    {
        List<Vector2> points = extractor.OutlinePoints;

        if (points == null || points.Count < 2)
        {
            Debug.LogWarning("OutlineVisualizer: Not enough points to draw.");
            return;
        }

        line.positionCount = points.Count;
        line.loop = loop;

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.useWorldSpace = false; // Draw relative to this GameObject

        // Apply points
        for (int i = 0; i < points.Count; i++)
        {
            line.SetPosition(i, points[i]);
        }

        Debug.Log("OutlineVisualizer: Outline drawn with " + points.Count + " points.");
    }
}
