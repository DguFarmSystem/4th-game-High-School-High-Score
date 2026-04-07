using System.Collections.Generic;
using UnityEngine;

public class OutlineExtractor
{
    private readonly Color32[] pixels;
    private readonly int width;
    private readonly int height;

    private readonly float alphaThreshold;

    public OutlineExtractor(Texture2D texture, float alphaThreshold = 0.1f)
    {
        this.width = texture.width;
        this.height = texture.height;
        this.pixels = texture.GetPixels32();
        this.alphaThreshold = alphaThreshold;
    }

    private bool IsSolid(int x, int y)
    {
        int index = y * width + x;
        return pixels[index].a / 255f >= alphaThreshold;
    }

    private bool IsOutlinePixel(int x, int y)
    {
        if (!IsSolid(x, y)) return false;

        int[] dirx = { 1, -1, 0, 0 };
        int[] diry = { 0, 0, 1, -1 };

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dirx[i];
            int ny = y + diry[i];

            if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                return true;

            if (!IsSolid(nx, ny))
                return true;
        }

        return false;
    }


    public List<Vector2> ExtractOutline()
    {
        List<Vector2> outlinePixels = new List<Vector2>();

        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                if (IsOutlinePixel(x, y))
                {
                    outlinePixels.Add(new Vector2(x, y));
                }
            }
        }

        // 가공된 외곽선(픽셀 순서가 섞여 있음) → 순차적 경로로 변환
        return SortOutlinePoints(outlinePixels);
    }

    private List<Vector2> SortOutlinePoints(List<Vector2> points)
    {
        if (points.Count == 0)
            return points;

        List<Vector2> sorted = new List<Vector2>();
        HashSet<Vector2> used = new HashSet<Vector2>();

        Vector2 current = points[0];
        sorted.Add(current);
        used.Add(current);

        for (int i = 1; i < points.Count; i++)
        {
            float minDist = float.MaxValue;
            Vector2 next = current;

            foreach (var p in points)
            {
                if (used.Contains(p)) continue;

                float dist = Vector2.SqrMagnitude(p - current);
                if (dist < minDist)
                {
                    minDist = dist;
                    next = p;
                }
            }

            sorted.Add(next);
            used.Add(next);
            current = next;
        }

        return sorted;
    }
}
