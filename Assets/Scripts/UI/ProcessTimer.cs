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
    // Start is called before the first frame update
    void Start()
    {
        //�������� �Ŵ������� �ð��� ������
        StageNormal manager = FindObjectOfType<StageNormal>();
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

        elapsedTime += Time.deltaTime;
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
