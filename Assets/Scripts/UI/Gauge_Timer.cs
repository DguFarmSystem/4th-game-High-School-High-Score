using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;
using UnityEngine.UI;

public class Gauge_Timer : MonoBehaviour
{
    public Image gaugeImage;   // ������ �̹��� (fillAmount�� ����)
    private float duration; // ���� �ð� (��)

    private float timeLeft;    // ���� �ð�
    private bool isRunning = false;
    // Start is called before the first frame update
    void Start()
    {
        StageNormal manager = FindFirstObjectByType<StageNormal>();
        duration = manager.TimerTime;
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
            timeLeft -= Time.deltaTime; // �����Ӹ��� �ð� ����
            // ���� �ð� ���� ���
            //float ratio = Mathf.Clamp01(timeLeft / duration);
            float ratio = timeLeft / duration;
            gaugeImage.fillAmount = ratio;

            // �ð��� �� �Ǹ� ����
            if (timeLeft <= 0f)
            {
                isRunning = false;
                gaugeImage.fillAmount = 0f; // ������ ������ ���
                OnTimerEnd();               // Ÿ�̸� ���� �� ó��
            }
        }
    }

    // Ÿ�̸� ���� �� ȣ��Ǵ� �Լ�
    void OnTimerEnd()
    {
        Debug.Log("Timer End");
        // ���⿡ �߰� ���� (��: ���� ����, ���� �ܰ� ��) �ֱ�
    }
}
