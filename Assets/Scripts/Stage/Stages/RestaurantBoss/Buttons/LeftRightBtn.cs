using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LeftRightBtn : MonoBehaviour
{
    public static int Combo { get; protected set; } = 0;

    protected virtual void OnDisable()
    {
        Combo = 0;
    }
}
