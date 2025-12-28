using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finger : MonoBehaviour
{
    [Header("Settings")]
    public float activeDuration = 2.3f;
    public float pulseSpeed = 10.0f;
    public float scaleRange = 0.2f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        StartCoroutine(PulseAndDestroy());
    }

    // 작업을 처리하는 코루틴 함수
    IEnumerator PulseAndDestroy()
    {
        float timer = 0f;

        while (timer < activeDuration)
        {
            timer += Time.deltaTime;
            float sineWave = Mathf.Sin(timer * pulseSpeed);
            float scaleMultiplier = 1.0f - (sineWave * scaleRange);

            transform.localScale = originalScale * scaleMultiplier;

            yield return null;
        }
        Destroy(gameObject);
    }
}