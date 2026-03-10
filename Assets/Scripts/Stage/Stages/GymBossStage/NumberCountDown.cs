using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberCountDown : MonoBehaviour
{
    [SerializeField]
    private BallMoving Outer;
    [SerializeField]
    private BallMoving Inner;

    private Text Display;
    private float TimeLimit = 30.0f;
    private bool deltaChanged = false;
    // Start is called before the first frame update
    void Start()
    {
        Display = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(TimeLimit > 0)
        {
            TimeLimit -= Time.deltaTime;
            Display.text = $"{(int)TimeLimit + 1}";
        }

        if ((int)TimeLimit % 10 != 0)
        {
            deltaChanged = false;
        }

        if ((int)TimeLimit % 10 == 0 && !deltaChanged)
        {
            Outer.deltaBallMoving -= 0.3f;
            Inner.deltaBallMoving -= 0.3f;
            deltaChanged = true;
        }
    }
}
