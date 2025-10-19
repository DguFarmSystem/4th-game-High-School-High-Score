using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GoldenSpongeItem : ConveyorItem
{
    public static int SpongeCount = 0;

    public Action OnInitialSpongeUsed;

    public override void OnRemovedFromConveyor()
    {
        OnInitialSpongeUsed?.Invoke();
    }

    private void Start()
    {
        SpongeCount--;
    }
}
