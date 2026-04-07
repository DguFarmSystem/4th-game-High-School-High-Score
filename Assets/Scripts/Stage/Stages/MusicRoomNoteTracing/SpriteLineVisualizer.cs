using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteLineVisualizer : MonoBehaviour
{
    public Image image;             // UI Image 컴포넌트 (Inspector에 할당)
    public float lineWidth = 0.02f;
    public Material lineMaterial;

    void Start()
    {
        if (image == null || image.sprite == null)
        {
            Debug.LogError("[SpriteLineVisualizer] Image 또는 Sprite가 지정되지 않았습니다.");
            return;
        }

        Sprite sprite = image.sprite;
        Texture2D tex = sprite.texture;

        Rect rect = sprite.textureRect;
        int w = (int)rect.width;
        int h = (int)rect.height;

        // 윤곽선 추출
        OutlineExtractor extractor = new OutlineExtractor(tex);
        List<Vector2> contour = extractor.ExtractOutline();

        Debug.Log($"총 윤곽선 점 개수: {contour.Count}");

        // 시각화
        VisualizeContour(contour, sprite, w, h);
    }

    // UI Image용 contour 시각화
    void VisualizeContour(List<Vector2> contour, Sprite sprite, int w, int h)
    {
        GameObject go = new GameObject("OutlineLine");
        go.transform.SetParent(transform.parent);  // UI 상에서 이미지와 같은 Canvas 아래에 둠

        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.positionCount = contour.Count + 1;
        lr.loop = false;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.useWorldSpace = true;

        if (lineMaterial != null)
            lr.material = lineMaterial;

        RectTransform rt = image.rectTransform;
        Vector2 size = rt.rect.size;                         // UI 이미지 크기 (px)
        Vector2 pivotOffset = rt.pivot * size;               // pivot 보정

        // 픽셀 → UI local → 월드좌표
        for (int i = 0; i < contour.Count; i++)
        {
            Vector2 pixelPos = contour[i];

            // Sprite 내부 픽셀 좌표 → Image 내부 local 좌표
            float localX = (pixelPos.x / w) * size.x - pivotOffset.x;
            float localY = (pixelPos.y / h) * size.y - pivotOffset.y;

            Vector3 uiLocalPos = new Vector3(localX, localY, 0);
            Vector3 worldPos = image.transform.TransformPoint(uiLocalPos);

            lr.SetPosition(i, worldPos);
        }

        // 마지막 점 → 첫 점 연결
        {
            Vector2 firstPix = contour[0];
            float fx = (firstPix.x / w) * size.x - pivotOffset.x;
            float fy = (firstPix.y / h) * size.y - pivotOffset.y;

            Vector3 uiLocalPos = new Vector3(fx, fy, 0);
            Vector3 worldPos = image.transform.TransformPoint(uiLocalPos);
            lr.SetPosition(contour.Count, worldPos);
        }
    }
}
