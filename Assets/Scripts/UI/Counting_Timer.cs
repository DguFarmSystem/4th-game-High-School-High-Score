using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counting_Timer : MonoBehaviour
{
    public Image numberImage;
    public Sprite[] numberSprites;
    public float duration = 10f;

    private float timeLeft;
    private bool isRunning = false;
    // Start is called before the first frame update
    void Start()
    {
        timeLeft = duration;
        isRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isRunning)
        {
            timeLeft -= Time.deltaTime;

            // 남은 시간에 따라 표시할 숫자 계산
            int currentNumber = Mathf.CeilToInt(timeLeft); // 2초마다 숫자 바꿈

            if (currentNumber >= 1 && currentNumber <= 5)
            {
                // numberSprites[0] = 1, [1] = 2 ... 이라고 가정
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
