using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayStageInitializer : MonoBehaviour
{
    [SerializeField] private GameObject LV1Stage;
    [SerializeField] private GameObject LV2Stage;
    [SerializeField] private GameObject LV3Stage;
    [SerializeField] private GameObject LV4Stage;

    void Awake()
    {
        int level = StageManager.Instance.GetDifficulty();

        switch (level)
        {
            case 1:
                LV1Stage.SetActive(true);
                break;

            case 2:
                LV2Stage.SetActive(true);
                break;

            case 3:
                LV3Stage.SetActive(true);
                break;

            case 4:
                LV4Stage.SetActive(true);
                break;

            default:
                Debug.LogError("Invalid stage level: " + level);
                break;
        }
    }
}
