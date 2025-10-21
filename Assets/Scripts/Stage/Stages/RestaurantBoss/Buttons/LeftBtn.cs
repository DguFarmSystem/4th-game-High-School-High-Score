using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LeftBtn : LeftRightBtn, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!selfButton.interactable || isFailedInput) return;

        otherButton.interactable = false; // 다른 버튼 비활성화
        ConveyorItem item = Conveyor.NextItem;
        if (item is PuddingItem or CakeItem or GoldenSpongeItem or TimerItem)
        {
            Conveyor.RemoveNextItem(false); // 왼쪽으로 이동
            if (item is not TimerItem)
            {
                CorrectInputCount++;
                Combo++;
            }
        }
        else
        {
            // 잘못된 아이템을 선택했을 때의 처리 (예: 효과음 재생, 점수 차감 등)
            Combo = 0;
            StartCoroutine(DisableButtonsTemporarily());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!selfButton.interactable || isFailedInput) return;

        otherButton.interactable = true; // 다시 활성화
    }
}
