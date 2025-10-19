using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RightBtn : LeftRightBtn, IPointerDownHandler, IPointerUpHandler
{
    public Button selfButton;
    public Button otherButton;

    private Conveyor _conveyor;

    public void OnPointerDown(PointerEventData eventData)
    {
        otherButton.interactable = false; // 다른 버튼 비활성화
        ConveyorItem item = _conveyor.NextItem;
        if (item is SpoonSetItem or CupItem or GoldenSpongeItem or TimerItem)
        {
            _conveyor.RemoveNextItem(true); // 오른쪽으로 이동
            Combo++;
        }
        else
        {
            // 잘못된 아이템을 선택했을 때의 처리 (예: 효과음 재생, 점수 차감 등)
            Combo = 0;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        otherButton.interactable = true; // 다시 활성화
    }

    private void Start()
    {
        _conveyor = FindObjectOfType<Conveyor>();
    }
}
