using UnityEngine;

/// <summary>
/// 동물 오브젝트에 붙이면 0.4초 간격으로 대각선 회전을 A ↔ B 뚝뚝 전환합니다.
/// 부드러운 보간 없이 즉시 스냅합니다.
/// </summary>
public class AnimalWiggle : MonoBehaviour
{
    [Header("뒤틀림 설정")]
    [Tooltip("전환 간격 (초)")]
    [SerializeField] private float wiggleInterval = 0.2f;

    [Tooltip("A 각도 (Z축 회전)")]
    [SerializeField] private float angleA = -7f;

    [Tooltip("B 각도 (Z축 회전)")]
    [SerializeField] private float angleB = 7f;

    private float timer = 0f;
    private bool isA = true;

    void Start()
    {
        // 시작하자마자 A 각도 적용
        transform.localRotation = Quaternion.Euler(0f, 0f, angleA);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wiggleInterval)
        {
            timer -= wiggleInterval;
            isA = !isA;
            transform.localRotation = Quaternion.Euler(0f, 0f, isA ? angleA : angleB);
        }
    }
}
