using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EraseTexture : MonoBehaviour
{
    [Header("���찳 ����")]
    public float eraseRadius = 20f;           // ���찳 �ݰ� (�ȼ� ����)
    public float erasedThreshold = 0.95f;     // ������ ���� ���� (0.95 = 95%)
    private Texture2D originalTexture;        // ���� �ؽ�ó
    private Texture2D editableTexture;        // ���� ������ �ؽ�ó
    private SpriteRenderer spriteRenderer;
    private int totalPixels;                  // ��ü �ȼ� ����
    private int erasedPixels;                 // ������ �ȼ� ����
    private bool isFullyErased = false;
    // �̹� ���� �ȼ� ��ǥ ���� (�ߺ� ī��Ʈ ����)
    private HashSet<Vector2Int> erasedPixelSet = new HashSet<Vector2Int>();
    public bool IsFullyErased => isFullyErased;
    public float ErasedRatio => (float)erasedPixels / totalPixels; // �ܺο����� ���� Ȯ�� ����
    public GameObject eraseCursorPrefab; // ���� ���찳 ǥ�ÿ� ������
    private GameObject eraseCursorInstance;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer�� �����ϴ�!");
            enabled = false;
            return;
        }
        // ���� �ؽ�ó �����ؼ� ���� �����ϰ� �����
        originalTexture = spriteRenderer.sprite.texture;
        editableTexture = Instantiate(originalTexture);
        editableTexture.Apply();
        // SpriteRenderer�� ���� ������ �ؽ�ó ����
        spriteRenderer.sprite = Sprite.Create(
            editableTexture,
            spriteRenderer.sprite.rect,
            spriteRenderer.sprite.pivot / spriteRenderer.sprite.rect.size,
            spriteRenderer.sprite.pixelsPerUnit
        );
        // �� �ȼ� �� ���
        totalPixels = 0;
        Color[] pixels = editableTexture.GetPixels();
        int width = editableTexture.width;
        int height = editableTexture.height;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color c = editableTexture.GetPixel(x, y);
                if (c.a > 0.01f)
                    totalPixels++;
            }
        }
    }
    void Update()
    {
        if (InputManager.IsPressing)
        {
            Vector3 pos = InputManager.TouchWorldPos;
            pos.z = 0f; // z=0 ��� ����
            EraseAt(pos);
            // �����
            Debug.DrawLine(Camera.main.transform.position, pos, Color.red, 0.05f);
            // ���찳 ��ġ ���� �� ǥ��
            if (eraseCursorInstance != null)
            {
                eraseCursorInstance.SetActive(true);
                eraseCursorInstance.transform.position = pos;
                eraseCursorInstance.transform.localScale =
                    Vector3.one * (eraseRadius * 2f / spriteRenderer.sprite.pixelsPerUnit);
            }
        }
        else
        {
            if (eraseCursorInstance != null)
                eraseCursorInstance.SetActive(false);
        }
    }
    void EraseAt(Vector3 worldPos)
    {
        // �ʵ�: spriteRenderer, editableTexture, eraseRadius, erasedPixelSet, erasedPixels, isFullyErased, ErasedRatio, erasedThreshold ���� ���� �ڵ� ���
        Vector3 localPos = spriteRenderer.transform.InverseTransformPoint(worldPos);
        Sprite sprite = spriteRenderer.sprite;
        if (sprite == null || editableTexture == null) return;
        Rect rect = sprite.rect; // sprite ���� rect (pixels)
        Vector4 border = sprite.border; // left, bottom, right, top (pixels)
        float ppu = sprite.pixelsPerUnit;
        Vector2 rendererSize = spriteRenderer.size; // world units
        // border�� ���� ������ ��ȯ
        float leftWorld = border.x / ppu;
        float bottomWorld = border.y / ppu;
        float rightWorld = border.z / ppu;
        float topWorld = border.w / ppu;
        // �߾�(�þ��) ������ ���� ����
        float centerWorldX = rendererSize.x - leftWorld - rightWorld;
        float centerWorldY = rendererSize.y - topWorld - bottomWorld;
        // �߾� ������ ���� �ȼ� ����
        float borderLeftPx = border.x;
        float borderRightPx = border.z;
        float borderBottomPx = border.y;
        float borderTopPx = border.w;
        float centerPxX = rect.width - borderLeftPx - borderRightPx;
        float centerPxY = rect.height - borderTopPx - borderBottomPx;
        // 9-slice�� ��Ȯ�� ������ �� �ִ� ���� (������: ��� �и� 0�̸� �ܼ� ��� fallback)
        bool useNineSliceX = (borderLeftPx > 0f || borderRightPx > 0f) && centerPxX > 0f && (centerWorldX > 0f);
        bool useNineSliceY = (borderBottomPx > 0f || borderTopPx > 0f) && centerPxY > 0f && (centerWorldY > 0f);
        // localPos�� pivot�� ����(0,0)���� �� �� left edge������ �Ÿ�(����) ����� ���� pivot�� '���� �Ÿ� from left/bottom'�� ���ؾ� ��.
        // pivot�� sprite.pivot (pixels, origin = bottom-left)
        float pivotPxX = sprite.pivot.x;
        float pivotPxY = sprite.pivot.y;
        // Helper: inverse mapping world(distanceFromLeft) -> pixel within rect (px from left)
        float PixelXFromLocalX(float lx)
        {
            // pivot�� left���� ������ ���� �Ÿ�
            float pivotWorldFromLeft;
            if (useNineSliceX)
            {
                // px -> world (from left): piecewise (left border / center / right border)
                System.Func<float, float> pixelToWorld = (px) =>
                {
                    if (px < borderLeftPx) // left border region
                        return (borderLeftPx <= 0f) ? 0f : (px / borderLeftPx) * leftWorld;
                    else if (px < borderLeftPx + centerPxX) // center region
                        return leftWorld + ((px - borderLeftPx) / centerPxX) * centerWorldX;
                    else // right border
                        return leftWorld + centerWorldX + ((px - (borderLeftPx + centerPxX)) / borderRightPx) * rightWorld;
                };
                pivotWorldFromLeft = pixelToWorld(pivotPxX);
            }
            else
            {
                // fallback: ��ü�� ���� �����Ϸ� ���
                pivotWorldFromLeft = (pivotPxX / rect.width) * rendererSize.x;
            }
            // left������ ���� ����Ÿ� = pivotWorldFromLeft + local x (local x�� pivot ����)
            float distanceFromLeft = pivotWorldFromLeft + lx;
            // clamp to sprite rendered width in world
            float totalWorldWidth = (useNineSliceX) ? (leftWorld + centerWorldX + rightWorld) : rendererSize.x;
            distanceFromLeft = Mathf.Clamp(distanceFromLeft, 0f, Mathf.Max(0.0001f, totalWorldWidth));
            // world -> pixel (inverse)
            if (useNineSliceX)
            {
                if (distanceFromLeft <= leftWorld)
                    return (leftWorld <= 0f) ? 0f : (distanceFromLeft / leftWorld) * borderLeftPx;
                else if (distanceFromLeft <= leftWorld + centerWorldX)
                    return borderLeftPx + ((distanceFromLeft - leftWorld) / centerWorldX) * centerPxX;
                else
                    return borderLeftPx + centerPxX + ((distanceFromLeft - leftWorld - centerWorldX) / rightWorld) * borderRightPx;
            }
            else
            {
                return (distanceFromLeft / rendererSize.x) * rect.width;
            }
        }
        float PixelYFromLocalY(float ly)
        {
            float pivotWorldFromBottom;
            if (useNineSliceY)
            {
                System.Func<float, float> pixelToWorld = (py) =>
                {
                    if (py < borderBottomPx) // bottom border
                        return (borderBottomPx <= 0f) ? 0f : (py / borderBottomPx) * bottomWorld;
                    else if (py < borderBottomPx + centerPxY) // center
                        return bottomWorld + ((py - borderBottomPx) / centerPxY) * centerWorldY;
                    else // top border
                        return bottomWorld + centerWorldY + ((py - (borderBottomPx + centerPxY)) / borderTopPx) * topWorld;
                };
                pivotWorldFromBottom = pixelToWorld(pivotPxY);
            }
            else
            {
                pivotWorldFromBottom = (pivotPxY / rect.height) * rendererSize.y;
            }
            float distanceFromBottom = pivotWorldFromBottom + ly;
            float totalWorldHeight = (useNineSliceY) ? (bottomWorld + centerWorldY + topWorld) : rendererSize.y;
            distanceFromBottom = Mathf.Clamp(distanceFromBottom, 0f, Mathf.Max(0.0001f, totalWorldHeight));
            if (useNineSliceY)
            {
                if (distanceFromBottom <= bottomWorld)
                    return (bottomWorld <= 0f) ? 0f : (distanceFromBottom / bottomWorld) * borderBottomPx;
                else if (distanceFromBottom <= bottomWorld + centerWorldY)
                    return borderBottomPx + ((distanceFromBottom - bottomWorld) / centerWorldY) * centerPxY;
                else
                    return borderBottomPx + centerPxY + ((distanceFromBottom - bottomWorld - centerWorldY) / topWorld) * borderTopPx;
            }
            else
            {
                return (distanceFromBottom / rendererSize.y) * rect.height;
            }
        }
        // ����� �ϴ� �ؽ�ó �� �ȼ� ��ǥ (float)
        float pixelXf = PixelXFromLocalX(localPos.x);
        float pixelYf = PixelYFromLocalY(localPos.y);
        // sprite.rect�� (x,y)�� �ؽ�ó ����(���� 0,0)������ �������̹Ƿ� ������� ���� �ؽ�ó ��ǥ�� �ȴ�.
        int centerTexX = Mathf.RoundToInt(rect.x + pixelXf);
        int centerTexY = Mathf.RoundToInt(rect.y + pixelYf);
        int radius = Mathf.CeilToInt(eraseRadius); // eraseRadius�� �ȼ� ������� ����
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                int px = centerTexX + x;
                int py = centerTexY + y;
                if (px >= 0 && px < editableTexture.width &&
                    py >= 0 && py < editableTexture.height)
                {
                    float distance = Mathf.Sqrt(x * x + y * y);
                    if (distance <= eraseRadius)
                    {
                        Vector2Int coord = new Vector2Int(px, py);
                        if (!erasedPixelSet.Contains(coord))
                        {
                            Color pixel = editableTexture.GetPixel(px, py);
                            if (pixel.a > 0.01f)
                            {
                                pixel.a = 0f;
                                editableTexture.SetPixel(px, py, pixel);
                                erasedPixelSet.Add(coord);
                                erasedPixels++;
                            }
                        }
                    }
                }
            }
        }
        editableTexture.Apply();
        CheckErased();
    }
    void CheckErased()
    {
        if (isFullyErased) return;
        float ratio = ErasedRatio;
        Debug.Log($"Erased Ratio: {ratio * 100f:F2}% / Threshold: {erasedThreshold * 100f:F2}%");
        if (ratio >= erasedThreshold - 0.0001f)
        {
            isFullyErased = true;
        }
    }
    // void OnFullyErased()
    // {
    //     Debug.Log($"{gameObject.name} �� ������ ������!");
    // }
}
