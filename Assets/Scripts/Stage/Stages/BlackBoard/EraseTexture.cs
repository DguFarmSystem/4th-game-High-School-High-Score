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
    // �ܺο����� ���� Ȯ�� ����
    public float ErasedRatio => (float)erasedPixels / totalPixels;

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

        //Sprite�� rect �������� �� �ȼ� �� ���
        totalPixels = 0;
        Color[] pixels = editableTexture.GetPixels();

        int width = editableTexture.width;
        int height = editableTexture.height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color c = editableTexture.GetPixel(x, y);
                if (c.a > 0.01f) // ���� ���� �̻� "������ �� ���"���� �Ǵ�
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

            //�����
            Debug.DrawLine(Camera.main.transform.position, pos, Color.red, 0.05f);

            // ���찳 ��ġ ���� �� ǥ��
            if (eraseCursorInstance != null)
            {
                eraseCursorInstance.SetActive(true);
                eraseCursorInstance.transform.position = pos;
                eraseCursorInstance.transform.localScale = Vector3.one * (eraseRadius * 2f / spriteRenderer.sprite.pixelsPerUnit);
                // eraseRadius�� �ȼ� ����, ����Ƽ ���� ������ ������
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

                //rect �� �ȼ��� ó��
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

        //�α� ��� �߰�
        Debug.Log($"Erased Ratio: {ratio * 100f:F1}%");

        if (ratio >= erasedThreshold)
        {
            isFullyErased = true;
            //OnFullyErased();
        }
    }

    //void OnFullyErased()
    //{
    //    Debug.Log($"{gameObject.name} �� ������ ������!");
    //    // ���⼭ ���ϴ� ó�� �߰�
    //    // Destroy(gameObject); // ������ �������� ����
    //    // �Ǵ� StageManager�� �˸� ������
    //}
}

