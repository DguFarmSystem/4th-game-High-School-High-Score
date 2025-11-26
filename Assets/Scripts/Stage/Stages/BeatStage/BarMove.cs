using System.Collections;
using UnityEngine;

public class BarMove : MonoBehaviour
{
    [Header("설정")]
    public float duration = 5f;    // 한 마디의 길이 (5초)
    public float disappearX = 10f; // 목표 지점 X좌표
    public float startDelay = 2.3f; // 시작 딜레이 (BeatStageManager와 맞춰야 함)

    private float speed;          // 내부에서 계산될 속도
    private float enableTime;     
    private bool isMoving = false;
    private float startX;         // 시작 위치 저장용

    void Awake()
    {
        // 오브젝트의 초기 위치를 시작점으로 잡습니다.
        startX = transform.position.x;
    }

    void OnEnable()
    {
        enableTime = Time.time;
        isMoving = false;
        
        // 켜질 때마다 현재 위치를 다시 잡거나, Awake의 값을 쓸 수 있습니다.
        // 만약 오브젝트가 재활용되면서 위치가 바뀐다면 여기서 startX를 다시 잡으세요.
        transform.position = new Vector3(startX, transform.position.y, transform.position.z);

        // 속도 자동 계산: (목표위치 - 시작위치) / 시간
        if (duration > 0)
        {
            float distance = disappearX - startX;
            speed = distance / duration;
        }
    }

    void Update()
    {
        if (!isMoving)
        {
            if (Time.time - enableTime >= startDelay)
            {
                isMoving = true;
            }
            else
            {
                return;
            }
        }

        // 계산된 속도로 이동
        transform.position += Vector3.right * speed * Time.deltaTime;

        // 목표 지점 도달 시 비활성화
        if (transform.position.x >= disappearX)
        {
            gameObject.SetActive(false); 
        }
    }
}