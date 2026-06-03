using UnityEngine;
using UnityEngine.UI;

public class UIAnimationHelper : MonoBehaviour
{
    private Image image;
    private Sprite lastSprite;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void LateUpdate()
    {
        if (image.sprite != lastSprite)
        {
            lastSprite = image.sprite;
            image.SetNativeSize();
        }
    }
}