using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerItem : ConveyorItem
{
    public override void OnRemovedFromConveyor()
    {
        Destroy(gameObject);
    }
}
