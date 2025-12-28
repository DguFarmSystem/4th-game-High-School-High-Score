using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRestaurantState : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StageManager.Instance.SetRestaurantCleared(true);
    }
}
