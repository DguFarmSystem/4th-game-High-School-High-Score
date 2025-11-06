using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    [SerializeField] private Image _timerFillImage;

    private RestaurantBossStage stage;

    private void Start()
    {
        stage = FindObjectOfType<RestaurantBossStage>();
    }
    private void Update()
    {
        if (stage != null)
        {
            float fillAmount = Mathf.Clamp01(stage.StageTimeLimit / 30f); // 최대 시간이 30초라고 가정
            _timerFillImage.fillAmount = fillAmount;
        }
    }
}
