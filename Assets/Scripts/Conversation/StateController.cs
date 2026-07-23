using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    public enum StageType { Tutorial, Restaurant, Music, Playground, HealthRoom }

    public StageType stageType;

    // Start is called before the first frame update
    void Start()
    {
        SetStageClear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetStageClear()
    {
        switch(stageType)
        {
            case StageType.Tutorial:
                {
                    DataManager.Instance.Player.SetTutorialCleared(true);
                    break;
                }
            case StageType.Restaurant:
                {
                    DataManager.Instance.Player.SetRestaurantCleared(true);
                    break;
                }
            case StageType.Music:
                {
                    DataManager.Instance.Player.SetMusicCleared(true);
                    break;
                }
            case StageType.Playground:
                {
                    DataManager.Instance.Player.SetPlaygroundCleared(true);
                    break;
                }
            case StageType.HealthRoom:
                {
                    DataManager.Instance.Player.SetHealthRoomCleared(true);
                    break;
                }
        }
    }
}
