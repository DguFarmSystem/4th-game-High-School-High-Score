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
        // ���� ��ǥ �� ���� ��ǥ
        Vector2 localPos = transform.InverseTransformPoint(worldPos);

        // ���� ��ǥ �� �ؽ�ó �ȼ� ��ǥ
        Rect spriteRect = spriteRenderer.sprite.rect;
        Vector2 pivot = spriteRenderer.sprite.pivot;
        float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;

        int texPosX = Mathf.RoundToInt(pivot.x + localPos.x * pixelsPerUnit);
        int texPosY = Mathf.RoundToInt(pivot.y + localPos.y * pixelsPerUnit);

        // ���찳 �ݰ� �� �ȼ� ���İ� ����
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
                            pixel.a = 0f; // ���� ����ȭ
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
        Debug.Log($"{gameObject.name} �� ������ ������!");
        // ���⼭ ���ϴ� ó�� �߰�
        // Destroy(gameObject); // ������ �������� ����
        // �Ǵ� StageManager�� �˸� ������
    }
}

