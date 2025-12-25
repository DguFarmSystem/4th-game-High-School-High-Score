using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelTouchHandler : MonoBehaviour, 
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    private int GuitarPlayCount = 0;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("터치가 패널 영역에 진입");
        ++GuitarPlayCount;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("터치가 패널 영역에서 이탈");
        ++GuitarPlayCount;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("패널 위에서 터치 시작");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("패널 위에서 터치 종료");
    }

    public int GetPlayCount()
    {
        return GuitarPlayCount;
    }
}
