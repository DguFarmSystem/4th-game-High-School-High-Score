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
            new Vector2(0.5f, 0.5f),
            spriteRenderer.sprite.pixelsPerUnit
        );

        totalPixels = editableTexture.width * editableTexture.height;
        erasedPixels = 0;
    }

    void Update()
    {
        if(InputManager.IsTouching)
        {
            EraseAt(InputManager.TouchWorldPos);
        }
    }

    void EraseAt(Vector3 worldPos)
    {
        // 월드 좌표 → 로컬 좌표
        Vector2 localPos = transform.InverseTransformPoint(worldPos);

        // 로컬 좌표 → 텍스처 픽셀 좌표
        Rect spriteRect = spriteRenderer.sprite.rect;
        Vector2 pivot = spriteRenderer.sprite.pivot;
        float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;

        int texPosX = Mathf.RoundToInt(pivot.x + localPos.x * pixelsPerUnit);
        int texPosY = Mathf.RoundToInt(pivot.y + localPos.y * pixelsPerUnit);

        // 지우개 반경 내 픽셀 알파값 수정
        int radius = Mathf.CeilToInt(eraseRadius);
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                int px = texPosX + x;
                int py = texPosY + y;

                if (px >= 0 && px < editableTexture.width && py >= 0 && py < editableTexture.height)
                {
                    float distance = Mathf.Sqrt(x * x + y * y);
                    if (distance <= eraseRadius)
                    {
                        Color pixel = editableTexture.GetPixel(px, py);
                        if (pixel.a > 0.01f)
                        {
                            pixel.a = 0f; // 완전 투명화
                            editableTexture.SetPixel(px, py, pixel);
                            erasedPixels++;
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

        float erasedRatio = (float)erasedPixels / totalPixels;
        if (erasedRatio >= erasedThreshold)
        {
            isFullyErased = true;
            OnFullyErased();
        }
    }

    void OnFullyErased()
    {
        Debug.Log($"{gameObject.name} → 완전히 지워짐!");
        // 여기서 원하는 처리 추가
        // Destroy(gameObject); // 완전히 지워지면 삭제
        // 또는 StageManager에 알림 보내기
    }
}

