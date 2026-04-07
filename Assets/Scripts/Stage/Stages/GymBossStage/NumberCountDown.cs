using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberCountDown : MonoBehaviour
{
    public bool Stop = false;

    private Text Display;
    private float TimeLimit = 30.0f;

    // Start is called before the first frame update
    void Start()
    {
        Display = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!Stop)
        {
            if (TimeLimit > 0)
            {
                TimeLimit -= Time.deltaTime;
                Display.text = $"{(int)TimeLimit + 1}";
            }
        }
    }
}
