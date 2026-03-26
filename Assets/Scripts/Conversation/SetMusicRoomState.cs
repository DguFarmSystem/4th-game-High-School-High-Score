using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMusicRoomState : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DataManager.Instance.Player.SetMusicCleared(true);
    }
}
