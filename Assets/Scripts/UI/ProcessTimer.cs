using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Stage;

public class ProcessTimer : MonoBehaviour
{
    [Header("Image Settings")]
    public Image targetImage; // 타이머에 표시할 Image
    public Sprite[] timerSprites; // 타이머 단계별 이미지 (6장)

    private float totalTime;
    private float elapsedTime = 0f;
    private int currentIndex = 0;
    private bool isRunning = false;
    // Start is called before the first frame update
    void Start()
    {
        //스테이지 매니저에서 시간을 가져옴
        StageNormal manager = FindObjectOfType<StageNormal>();
        totalTime = manager.TimerTime;
        // 시작 시 초기 이미지 설정
        if (timerSprites.Length > 0 && targetImage != null)
            targetImage.sprite = timerSprites[0];
        StartTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;
        float ratio = elapsedTime / totalTime;

        // 현재 인덱스 계산 (0 ~ sprites.Length-1)
        int index = Mathf.FloorToInt(ratio * timerSprites.Length);

        // 인덱스가 범위를 넘어가지 않도록 보정
        index = Mathf.Clamp(index, 0, timerSprites.Length - 1);

        if (index != currentIndex)
        {
            currentIndex = index;
            targetImage.sprite = timerSprites[currentIndex];
        }

        // 시간이 다 되었을 때 타이머 정지
        if (elapsedTime >= totalTime)
        {
            isRunning = false;
        }
    }

    public void StartTimer()
    {
        elapsedTime = 0f;
        currentIndex = 0;
        isRunning = true;

        if (timerSprites.Length > 0 && targetImage != null)
            targetImage.sprite = timerSprites[0];
    }
}
