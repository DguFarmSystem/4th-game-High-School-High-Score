using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;
using UnityEngine.UI;

public class Counting_Timer : MonoBehaviour
{
    public Image numberImage;
    public Sprite[] numberSprites;
    private float duration;

    private float timeLeft;
    private bool isRunning = false;
    // Start is called before the first frame update
    void Start()
    {
        StageNormal manager = FindObjectOfType<StageNormal>();
        duration = manager.TimerTime;
        timeLeft = duration;
        isRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isRunning)
        {
            timeLeft -= Time.deltaTime;

            // ���� �ð��� ���� ǥ���� ���� ���
            int currentNumber = Mathf.CeilToInt(timeLeft); // 2�ʸ��� ���� �ٲ�

            if (currentNumber >= 1 && currentNumber <= 5)
            {
                // numberSprites[0] = 1, [1] = 2 ... �̶�� ����
                numberImage.sprite = numberSprites[currentNumber - 1];
            }

            if (timeLeft <= 0f)
            {
                isRunning = false;
                OnTimerEnd();
            }
        }
    }

    void OnTimerEnd()
    {

    }
}
