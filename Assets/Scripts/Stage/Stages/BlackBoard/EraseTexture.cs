using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraseTexture : MonoBehaviour
{
    [Header("지우개 설정")]
    public float eraseRadius = 20f;           // 지우개 반경 (픽셀 단위)
    public float erasedThreshold = 0.95f;     // 지워진 판정 비율 (0.95 = 95%)

    private Texture2D originalTexture;        // 원본 텍스처
    private Texture2D editableTexture;        // 수정 가능한 텍스처
    private SpriteRenderer spriteRenderer;

    private int totalPixels;                  // 전체 픽셀 개수
    private int erasedPixels;                 // 지워진 픽셀 개수
    private bool isFullyErased = false;

    // 이미 지운 픽셀 좌표 저장 (중복 카운트 방지)
    private HashSet<Vector2Int> erasedPixelSet = new HashSet<Vector2Int>();
    public bool IsFullyErased => isFullyErased;
    public float ErasedRatio => (float)erasedPixels / totalPixels; // 외부에서도 비율 확인 가능

    public GameObject eraseCursorPrefab; // 원형 지우개 표시용 프리팹
    private GameObject eraseCursorInstance;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer가 없습니다!");
            enabled = false;
            return;
        }

        // 원본 텍스처 복사해서 수정 가능하게 만들기
        originalTexture = spriteRenderer.sprite.texture;
        editableTexture = Instantiate(originalTexture);
        editableTexture.Apply();

        // SpriteRenderer에 수정 가능한 텍스처 적용
        spriteRenderer.sprite = Sprite.Create(
            editableTexture,
            spriteRenderer.sprite.rect,
            spriteRenderer.sprite.pivot / spriteRenderer.sprite.rect.size,
            spriteRenderer.sprite.pixelsPerUnit
        );

        // 총 픽셀 수 계산
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
            pos.z = 0f; // z=0 평면 고정
            EraseAt(pos);

            // 디버그
            Debug.DrawLine(Camera.main.transform.position, pos, Color.red, 0.05f);

            // 지우개 위치 갱신 및 표시
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
        // 필드: spriteRenderer, editableTexture, eraseRadius, erasedPixelSet, erasedPixels, isFullyErased, ErasedRatio, erasedThreshold 등은 기존 코드 사용
        Vector3 localPos = spriteRenderer.transform.InverseTransformPoint(worldPos);
        Sprite sprite = spriteRenderer.sprite;
        if (sprite == null || editableTexture == null) return;

        Rect rect = sprite.rect; // sprite 내부 rect (pixels)
        Vector4 border = sprite.border; // left, bottom, right, top (pixels)
        float ppu = sprite.pixelsPerUnit;
        Vector2 rendererSize = spriteRenderer.size; // world units

        // border를 월드 단위로 변환
        float leftWorld = border.x / ppu;
        float bottomWorld = border.y / ppu;
        float rightWorld = border.z / ppu;
        float topWorld = border.w / ppu;

        // 중앙(늘어나는) 영역의 월드 길이
        float centerWorldX = rendererSize.x - leftWorld - rightWorld;
        float centerWorldY = rendererSize.y - topWorld - bottomWorld;

        // 중앙 영역의 원래 픽셀 길이
        float borderLeftPx = border.x;
        float borderRightPx = border.z;
        float borderBottomPx = border.y;
        float borderTopPx = border.w;
        float centerPxX = rect.width - borderLeftPx - borderRightPx;
        float centerPxY = rect.height - borderTopPx - borderBottomPx;

        // 9-slice로 정확히 맵핑할 수 있는 조건 (안전성: 모든 분모가 0이면 단순 비례 fallback)
        bool useNineSliceX = (borderLeftPx > 0f || borderRightPx > 0f) && centerPxX > 0f && (centerWorldX > 0f);
        bool useNineSliceY = (borderBottomPx > 0f || borderTopPx > 0f) && centerPxY > 0f && (centerWorldY > 0f);

        // localPos는 pivot을 원점(0,0)으로 함 → left edge에서의 거리(월드) 계산을 위해 pivot의 '월드 거리 from left/bottom'을 구해야 함.
        // pivot은 sprite.pivot (pixels, origin = bottom-left)
        float pivotPxX = sprite.pivot.x;
        float pivotPxY = sprite.pivot.y;

        // Helper: inverse mapping world(distanceFromLeft) -> pixel within rect (px from left)
        float PixelXFromLocalX(float lx)
        {
            // pivot이 left에서 떨어진 월드 거리
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
                // fallback: 전체를 균일 스케일로 취급
                pivotWorldFromLeft = (pivotPxX / rect.width) * rendererSize.x;
            }

            // left에서의 실제 월드거리 = pivotWorldFromLeft + local x (local x는 pivot 기준)
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

        // 얻고자 하는 텍스처 내 픽셀 좌표 (float)
        float pixelXf = PixelXFromLocalX(localPos.x);
        float pixelYf = PixelYFromLocalY(localPos.y);

        // sprite.rect의 (x,y)은 텍스처 원점(보통 0,0)에서의 오프셋이므로 더해줘야 실제 텍스처 좌표가 된다.
        int centerTexX = Mathf.RoundToInt(rect.x + pixelXf);
        int centerTexY = Mathf.RoundToInt(rect.y + pixelYf);

        int radius = Mathf.CeilToInt(eraseRadius); // eraseRadius가 픽셀 단위라고 가정
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
    //     Debug.Log($"{gameObject.name} → 완전히 지워짐!");
    // }
}
