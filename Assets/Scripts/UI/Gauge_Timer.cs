using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gauge_Timer : MonoBehaviour
{
    public Image gaugeImage;   // 게이지 이미지 (fillAmount를 줄임)
    public float duration = 10f; // 제한 시간 (초)

    private float timeLeft;    // 남은 시간
    private bool isRunning = false;
    // Start is called before the first frame update
    void Start()
    {
        timeLeft = duration;
        gaugeImage.fillAmount = 1f;
        isRunning = true;
        Debug.Log("Timer Start!");
    }

    // Update is called once per frame
    void Update()
    {
        if(isRunning)
        {
            timeLeft -= Time.deltaTime; // 프레임마다 시간 감소
            Debug.Log("gauge decreased");
            // 남은 시간 비율 계산
            //float ratio = Mathf.Clamp01(timeLeft / duration);
            float ratio = Mathf.Ceil(timeLeft / duration * 10f) / 10f;
            gaugeImage.fillAmount = ratio;

            // 시간이 다 되면 멈춤
            if (timeLeft <= 0f)
            {
                isRunning = false;
                gaugeImage.fillAmount = 0f; // 게이지 완전히 비움
                OnTimerEnd();               // 타이머 종료 시 처리
            }
        }
    }

    // 타이머 종료 시 호출되는 함수
    void OnTimerEnd()
    {
        Debug.Log("Timer End");
        // 여기에 추가 동작 (예: 게임 오버, 다음 단계 등) 넣기
    }
}
