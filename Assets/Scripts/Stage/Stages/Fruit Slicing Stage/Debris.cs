using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
{
    float a = 0;

    void Start()
    {
        
    }

    void Update()
    {
        if(a>0.05f)GetComponent<BoxCollider2D>().enabled = false;
        else a += Time.deltaTime;
    }
}
