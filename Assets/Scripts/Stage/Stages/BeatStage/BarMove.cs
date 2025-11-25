using System.Collections;
using UnityEngine;

public class BarMove : MonoBehaviour
{
    public float speed = 2f; // 이동 속도 (초당 단위)
    public float disappearX = 10f; // 이 위치에 도달하면 사라짐

    private float startDelay = 2.3f;
    private float startTime;
    private bool started = false;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (!started)
        {
            if (Time.time - startTime >= startDelay)
                started = true;
            else
                return;
        }

        // 2.3초 후부터 이동 시작
        transform.position += Vector3.right * speed * Time.deltaTime;

        // 특정 위치에 도달하면 오브젝트 비활성화 또는 파괴
        if (transform.position.x >= disappearX)
        {
            gameObject.SetActive(false); // 또는 Destroy(gameObject);
        }
    }
}