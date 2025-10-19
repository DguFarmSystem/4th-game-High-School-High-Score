using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupItem : ConveyorItem
{
   public override void OnRemovedFromConveyor()
    {
        // 컵 아이템이 컨베이어에서 제거될 때의 동작 구현
        // 예: 콤보 UI 업데이트, 효과음 재생 등
    }
}
