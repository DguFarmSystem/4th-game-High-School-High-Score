using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickChaser : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Transform Pickpos = this.GetComponent<Transform>();
        Pickpos.position = InputManager.Instance.TouchWorldPos;
    }
}
