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
    // 외부에서도 비율 확인 가능
    public float ErasedRatio => (float)erasedPixels / totalPixels;

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
        Vector2 originalPivot = new Vector2(
         spriteRenderer.sprite.pivot.x / spriteRenderer.sprite.rect.width,
         spriteRenderer.sprite.pivot.y / spriteRenderer.sprite.rect.height
     );

        spriteRenderer.sprite = Sprite.Create(
            editableTexture,
            spriteRenderer.sprite.rect,
            originalPivot,
            spriteRenderer.sprite.pixelsPerUnit
        );

        //Sprite의 rect 기준으로 총 픽셀 수 계산
        totalPixels = 0;
        Color[] pixels = editableTexture.GetPixels();

        int width = editableTexture.width;
        int height = editableTexture.height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color c = editableTexture.GetPixel(x, y);
                if (c.a > 0.01f) // 일정 알파 이상만 "지워야 할 대상"으로 판단
                    totalPixels++;
            }
        }
    }

    void Update()
    {
        if (InputManager.IsPressing)
        {
            Vector3 pos = InputManager.TouchWorldPos;
            EraseAt(pos);

            //디버그
            Debug.DrawLine(Camera.main.transform.position, pos, Color.red, 0.05f);

            // 지우개 위치 갱신 및 표시
            if (eraseCursorInstance != null)
            {
                eraseCursorInstance.SetActive(true);
                eraseCursorInstance.transform.position = pos;
                eraseCursorInstance.transform.localScale = Vector3.one * (eraseRadius * 2f / spriteRenderer.sprite.pixelsPerUnit);
                // eraseRadius는 픽셀 단위, 유니티 월드 단위로 맞춰줌
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
        Vector2 localPos = transform.InverseTransformPoint(worldPos);

        Rect spriteRect = spriteRenderer.sprite.rect;
        Vector2 pivot = spriteRenderer.sprite.pivot;
        float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;

        int texPosX = Mathf.RoundToInt(pivot.x + localPos.x * pixelsPerUnit);
        int texPosY = Mathf.RoundToInt(pivot.y + localPos.y * pixelsPerUnit);

        int radius = Mathf.CeilToInt(eraseRadius);

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                int px = texPosX + x;
                int py = texPosY + y;

                //rect 내 픽셀만 처리
                if (px >= spriteRect.xMin && px < spriteRect.xMax &&
                    py >= spriteRect.yMin && py < spriteRect.yMax)
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

        //로그 출력 추가
        Debug.Log($"Erased Ratio: {ratio * 100f:F1}%");

        if (ratio >= erasedThreshold)
        {
            isFullyErased = true;
            //OnFullyErased();
        }
    }

    //void OnFullyErased()
    //{
    //    Debug.Log($"{gameObject.name} → 완전히 지워짐!");
    //    // 여기서 원하는 처리 추가
    //    // Destroy(gameObject); // 완전히 지워지면 삭제
    //    // 또는 StageManager에 알림 보내기
    //}
}

