using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineExtractor : MonoBehaviour
{
    public Sprite sprite;
    public float alphaThreshold = 0.1f; // Transparent cutoff
    public float simplifyTolerance = 2f; // Point reduction

    public List<Vector2> OutlinePoints = new List<Vector2>();

    void Start()
    {
        sprite = GetComponent<Image>().sprite;
        if (sprite == null)
        {
            Debug.LogError("No sprite assigned.");
            return;
        }

        GenerateOutline();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateOutline()
    {
        Texture2D tex = sprite.texture;

        int width = tex.width;
        int height = tex.height;

        List<Vector2> rawPoints = new List<Vector2>();

        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                Color c = tex.GetPixel(x, y);

                // Skip empty pixels
                if (c.a < alphaThreshold)
                    continue;

                // Check if this pixel touches transparency
                bool isEdge =
                    tex.GetPixel(x - 1, y).a < alphaThreshold ||
                    tex.GetPixel(x + 1, y).a < alphaThreshold ||
                    tex.GetPixel(x, y - 1).a < alphaThreshold ||
                    tex.GetPixel(x, y + 1).a < alphaThreshold;

                if (isEdge)
                {
                    // Convert texture pixel to LOCAL sprite space
                    Vector2 p = PixelToLocalPoint(x, y, width, height);
                    rawPoints.Add(p);
                }
            }
        }

        // Optional: simplify
        OutlinePoints = DouglasPeuckerReduction(rawPoints, simplifyTolerance);

        Debug.Log($"Generated outline: {OutlinePoints.Count} points");
    }

    Vector2 PixelToLocalPoint(int x, int y, int w, int h)
    {
        // Convert to 0–1 UV
        float px = (float)x / w;
        float py = (float)y / h;

        // Convert UV → local sprite bounds
        Vector2 local = new Vector2(
            Mathf.Lerp(sprite.bounds.min.x, sprite.bounds.max.x, px),
            Mathf.Lerp(sprite.bounds.min.y, sprite.bounds.max.y, py)
        );

        return local;
    }


    // ================================
    // Simplification (Ramer–Douglas–Peucker)
    // ================================

    public static List<Vector2> DouglasPeuckerReduction(List<Vector2> points, float tolerance)
    {
        if (points == null || points.Count < 3)
            return points;

        int firstPoint = 0;
        int lastPoint = points.Count - 1;

        List<int> pointIndexsToKeep = new List<int>();

        // Add first and last index
        pointIndexsToKeep.Add(firstPoint);
        pointIndexsToKeep.Add(lastPoint);

        // Recursively find points
        DouglasPeucker(points, firstPoint, lastPoint, tolerance, ref pointIndexsToKeep);

        // Sort and return simplified list
        pointIndexsToKeep.Sort();
        List<Vector2> finalPoints = new List<Vector2>();
        foreach (int idx in pointIndexsToKeep)
            finalPoints.Add(points[idx]);

        return finalPoints;
    }

    static void DouglasPeucker(List<Vector2> points, int firstPoint, int lastPoint, float tolerance, ref List<int> keep)
    {
        float maxDistance = 0;
        int indexFarthest = 0;

        for (int i = firstPoint + 1; i < lastPoint; i++)
        {
            float dist = PerpendicularDistance(points[firstPoint], points[lastPoint], points[i]);
            if (dist > maxDistance)
            {
                maxDistance = dist;
                indexFarthest = i;
            }
        }

        if (maxDistance > tolerance)
        {
            keep.Add(indexFarthest);

            // Recursive
            DouglasPeucker(points, firstPoint, indexFarthest, tolerance, ref keep);
            DouglasPeucker(points, indexFarthest, lastPoint, tolerance, ref keep);
        }
    }

    static float PerpendicularDistance(Vector2 start, Vector2 end, Vector2 p)
    {
        float area = Mathf.Abs(
            (start.x * end.y + end.x * p.y + p.x * start.y)
            - (start.y * end.x + end.y * p.x + p.y * start.x)
        );
        float bottom = Vector2.Distance(start, end);
        return area / bottom * 2;
    }
}
