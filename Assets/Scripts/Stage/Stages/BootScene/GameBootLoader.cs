using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBootLoader : MonoBehaviour
{
    // ===== Lifecycle methods ===== //
    void Start()
    {
        LoadingSceneController.Instance.LoadScene(SceneNames.Main/*, () => DataManager.Instance.LoadData()*/);
    }
}
