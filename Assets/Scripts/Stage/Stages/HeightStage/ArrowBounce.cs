using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBounce : MonoBehaviour
{
    [Header("Destroy Settings")]
    [Tooltip("오브젝트가 파괴되기까지의 시간 (초)")]
    public float destroyTime = 2.3f;

    [Header("Pulse Settings")]
    [Tooltip("커졌다 작아지는 속도")]
    public float pulseSpeed = 7.5f;
    [Tooltip("크기가 변하는 정도")]
    public float pulseAmount = 0.1f;

    private Vector3 originalScale;
    private float timer = 0f;

    void Start()
    {
        // 처음 크기 저장
        originalScale = transform.localScale;

        // 지정된 시간(2.3초) 뒤에 게임 오브젝트 파괴
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Mathf.Sin을 사용해 부드럽게 커졌다 작아지도록 연산 (-1 ~ 1 반복)
        float scaleMultiplier = 1.0f + (Mathf.Sin(timer * pulseSpeed) * pulseAmount);

        // 현재 스케일에 적용
        transform.localScale = originalScale * scaleMultiplier;
    }
}