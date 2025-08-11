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
            new Vector2(0.5f, 0.5f), // pivot �߽�
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
        // ���� ��ǥ �� ���� ��ǥ ��ȯ
        Vector3 localPos = spriteRenderer.transform.InverseTransformPoint(worldPos);

        // pivot(0.5, 0.5) �߽� ���� �� �ȼ� ��ǥ ��ȯ
        float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;
        int pixelX = Mathf.RoundToInt(localPos.x * pixelsPerUnit + editableTexture.width / 2f);
        int pixelY = Mathf.RoundToInt(localPos.y * pixelsPerUnit + editableTexture.height / 2f);

        int radius = Mathf.CeilToInt(eraseRadius);

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                int px = pixelX + x;
                int py = pixelY + y;

                // �ؽ�ó ���� ��
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
