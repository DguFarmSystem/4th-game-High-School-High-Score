using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMusicRoomState : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StageManager.Instance.SetMusicCleared(true);
    }
}
