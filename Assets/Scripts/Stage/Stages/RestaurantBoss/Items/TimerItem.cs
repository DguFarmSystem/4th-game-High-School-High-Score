using System.Collections;
using System.Collections.Generic;
using Stage;
using UnityEngine;

public class TimerItem : ConveyorItem
{
    RestaurantBossStage stage;
    public override void OnRemovedFromConveyor()
    {
        if (stage.CurrentState == StageState.Playing) stage.GetExtraTime(5f); // 타이머 아이템을 제거할 때
        Destroy(gameObject);
    }

    protected void Start()
    {
        stage = FindObjectOfType<RestaurantBossStage>();
    }
}
