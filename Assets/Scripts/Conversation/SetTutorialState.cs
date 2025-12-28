using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTutorialState : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StageManager.Instance.SetTutorialCleared(true);
    }
}
