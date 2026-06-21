using System.Collections;
using UnityEngine;

public class FingerPulse : MonoBehaviour
{
    [Header("Scale Settings")]
    [SerializeField] private float scaleAmount = 0.15f;
    [SerializeField] private float speed = 3f;

    [Header("Hide Timing")]
    [SerializeField] private float hideAfterSeconds = 1.8f;

    private Vector3 baseScale;

    private void OnEnable()
    {
        baseScale = transform.localScale;
        StartCoroutine(HideAfterDelay());
    }

    private void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        float scale = 1f + (scaleAmount * t);
        transform.localScale = baseScale * scale;
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideAfterSeconds);
        gameObject.SetActive(false);
    }
}