using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Stage;

public class ProcessTimer : MonoBehaviour
{
    [Header("Image Settings")]
    public Image targetImage; // Ÿ�̸ӿ� ǥ���� Image
    public Sprite[] timerSprites; // Ÿ�̸� �ܰ躰 �̹��� (6��)

    private float totalTime;
    private float elapsedTime = 0f;
    private int currentIndex = 0;
    private bool isRunning = false;
    private StageNormal manager;

    private bool audioFlag = false;
    private AudioClip normalClip;
    private AudioClip fastClip;
    // Start is called before the first frame update
    void Start()
    {
        normalClip = Resources.Load<AudioClip>("Timer/Timer_5-3");
        fastClip = Resources.Load<AudioClip>("Timer/Timer_2-1");
        //�������� �Ŵ������� �ð��� ������
        manager = FindObjectOfType<StageNormal>();
        totalTime = manager.TimerTime;
        // ���� �� �ʱ� �̹��� ����
        if (timerSprites.Length > 0 && targetImage != null)
            targetImage.sprite = timerSprites[0];
        StartTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning) return;

        elapsedTime = totalTime - manager.TimerTime;
        float ratio = elapsedTime / totalTime;

        // ���� �ε��� ��� (0 ~ sprites.Length-1)
        int index = Mathf.FloorToInt(ratio * timerSprites.Length);

        // �ε����� ������ �Ѿ�� �ʵ��� ����
        index = Mathf.Clamp(index, 0, timerSprites.Length - 1);

        if (index != currentIndex)
        {
            currentIndex = index;
            targetImage.sprite = timerSprites[currentIndex];
        }

        // �ð��� �� �Ǿ��� �� Ÿ�̸� ����
        if (elapsedTime >= totalTime)
        {
            isRunning = false;
        }

        if (!audioFlag && manager.TimerTime > 4f && manager.TimerTime < 5f)
            {
                SoundManager.Instance.PlayGaugeSound(normalClip);
                audioFlag = true;
            }

            if (manager.TimerTime < 4f && manager.TimerTime > 3f)
            {
                audioFlag = false;
            }

            if (!audioFlag && manager.TimerTime <= 3f)
            {
                SoundManager.Instance.PlayGaugeSound(fastClip);
                audioFlag = true;
            }

            if (manager.TimerTime <= 0f)
            {
                SoundManager.Instance.StopGaugeSound();
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
