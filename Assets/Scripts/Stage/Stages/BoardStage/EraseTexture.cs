using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EraseTexture : MonoBehaviour
{
    [Header("Erase Settings")]
    public float eraseRadius = 50f; // ȭ�� ���� ���찳 �ݰ� (World units)

    private SpriteRenderer spriteRenderer;
    private Texture2D runtimeTex;
    private int totalPixels;
    private int erasedPixels;

    private int texWidth;
    private int texHeight;
    private bool[,] erasedMask; // ȭ�� ���� ������� ���� ���� ���

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ���� Texture ���� (������ ��ȣ)
        Texture2D srcTex = spriteRenderer.sprite.texture;
        Rect spriteRect = spriteRenderer.sprite.rect;

        runtimeTex = new Texture2D((int)spriteRect.width, (int)spriteRect.height, srcTex.format, false);
        runtimeTex.SetPixels(srcTex.GetPixels(
            (int)spriteRect.x,
            (int)spriteRect.y,
            (int)spriteRect.width,
            (int)spriteRect.height
        ));
        runtimeTex.Apply();

        // Sprite�� ���� �����Ͽ� Renderer�� ����
        spriteRenderer.sprite = Sprite.Create(runtimeTex,
            new Rect(0, 0, runtimeTex.width, runtimeTex.height),
            spriteRenderer.sprite.pivot / new Vector2(spriteRect.width, spriteRect.height),
            spriteRenderer.sprite.pixelsPerUnit);

        texWidth = runtimeTex.width;
        texHeight = runtimeTex.height;

        totalPixels = texWidth * texHeight;
        erasedPixels = 0;

        erasedMask = new bool[texWidth, texHeight];
    }

    void Update()
    {
        if (!InputManager.IsPressing) return;

        Vector3 touchWorldPos = InputManager.TouchWorldPos;

        // Sprite local ��ǥ
        Vector3 localPos = transform.InverseTransformPoint(touchWorldPos);

        // Sprite size (World units)
        Vector2 spriteSize = new Vector2(
            spriteRenderer.sprite.bounds.size.x * transform.localScale.x,
            spriteRenderer.sprite.bounds.size.y * transform.localScale.y
        );

        // World localPos �� Texture UV ��ǥ
        float uvX = (localPos.x / spriteSize.x + 0.5f);
        float uvY = (localPos.y / spriteSize.y + 0.5f);

        int centerX = Mathf.RoundToInt(uvX * texWidth);
        int centerY = Mathf.RoundToInt(uvY * texHeight);

        int radiusPixelsX = Mathf.RoundToInt(eraseRadius / spriteSize.x * texWidth);
        int radiusPixelsY = Mathf.RoundToInt(eraseRadius / spriteSize.y * texHeight);

        // Texture ���� ������ ���� ���� �����
        for (int y = -radiusPixelsY; y <= radiusPixelsY; y++)
        {
            for (int x = -radiusPixelsX; x <= radiusPixelsX; x++)
            {
                int tx = centerX + x;
                int ty = centerY + y;

                if (tx >= 0 && tx < texWidth && ty >= 0 && ty < texHeight)
                {
                    if (x * x + y * y <= Mathf.Max(radiusPixelsX, radiusPixelsY) * Mathf.Max(radiusPixelsX, radiusPixelsY))
                    {
                        if (!erasedMask[tx, ty])
                        {
                            erasedMask[tx, ty] = true;
                            erasedPixels++;
                            runtimeTex.SetPixel(tx, ty, new Color(0, 0, 0, 0));
                        }
                    }
                }
            }
        }

        runtimeTex.Apply();
    }

    public float GetErasedRatio()
    {
        return Mathf.Clamp01((float)erasedPixels / totalPixels);
    }
}
