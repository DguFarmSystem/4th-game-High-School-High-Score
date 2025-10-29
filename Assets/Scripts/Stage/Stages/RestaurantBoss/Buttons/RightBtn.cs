using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightBtn : LeftRightBtn, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!selfButton.interactable || isFailedInput) return;

        otherButton.interactable = false; // 다른 버튼 비활성화
        ConveyorItem item = Conveyor.NextItem;
        if (item is SpoonSetItem or CupItem or GoldenSpongeItem or TimerItem)
        {
            Conveyor.RemoveNextItem(true); // 오른쪽으로 이동
            if (criticalTimer >= 0f)
            {
                Instantiate(criticalEffect, item.transform.position + Vector3.up, Quaternion.identity);
                if (criticalImage.enabled == false)
                {
                    Vector2 anchoredPos;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        criticalImage.transform.parent as RectTransform, // 부모 UI의 RectTransform
                        Camera.main.WorldToScreenPoint(item.transform.position + Vector3.up),  // 3D 오브젝트의 World 좌표를 Screen 좌표로 변환
                        null,                               // 사용 중인 카메라
                        out anchoredPos                            // 변환된 UI 로컬 좌표
                    );
                    criticalImage.rectTransform.anchoredPosition = anchoredPos;

                    criticalImage.enabled = true;
                }
            }

            criticalTimer = criticalDuration;

            if (item is not TimerItem)
            {
                CorrectInputCount++;
                Combo++;
                comboNotifier.GetComponentInChildren<TextMeshProUGUI>().text = $"{Combo}";
                comboNotifier.SetActive(true);

                goalLeftText.text = $"목표까지 {RemainingItemCount}개";
            }
        }
        else
        {
            // 잘못된 아이템을 선택했을 때의 처리 (예: 효과음 재생, 점수 차감 등)
            Combo = 0;
            comboNotifier.SetActive(false);
            criticalTimer = 0f;
            StartCoroutine(DisableButtonsTemporarily());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!selfButton.interactable || isFailedInput) return;

        otherButton.interactable = true; // 다시 활성화
    }
}
