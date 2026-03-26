using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRestaurantState : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DataManager.Instance.Player.SetRestaurantCleared(true);
    }
}
