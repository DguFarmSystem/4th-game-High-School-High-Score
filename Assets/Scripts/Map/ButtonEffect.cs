using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEffect : MonoBehaviour
{
    MapController controller;
    [SerializeField]
    private Sprite changeSprite;
    public float Upscale = 1.2f;
    public float speed = 3f;

    private bool onClicked = false;
    private Image buttonImage;
    private Sprite originalSprite;
    private Vector3 originalScale;
    private Coroutine scaleCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        buttonImage = GetComponent<Image>();
        originalSprite = buttonImage.sprite;
        originalScale = transform.localScale;
        if (controller == null)
        {
            controller = FindObjectOfType<MapController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null) return;

        if (onClicked)
        {
            //UI Image 교체
            buttonImage.sprite = changeSprite;
            // UI가 켜졌으면 커진 상태로 유지
            if (scaleCoroutine == null)
            {
                scaleCoroutine = StartCoroutine(ScaleTo(originalScale * Upscale, 1f / speed));
            }
        }
        else
        {
            //UI Image 원상복구
            buttonImage.sprite = originalSprite;
            // UI가 꺼졌으면 원래 크기로 복귀
            if (scaleCoroutine != null)
            {
                StopCoroutine(scaleCoroutine);
                scaleCoroutine = null;
            }
            StartCoroutine(ScaleTo(originalScale, 0.1f));
        }
    }

    public void toggleButton()
    {
        onClicked = !onClicked;
    }

    private IEnumerator ScaleTo(Vector3 target, float time)
    {
        Vector3 start = transform.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / time;
            transform.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.localScale = target;
    }
}
