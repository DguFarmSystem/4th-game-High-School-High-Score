using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    float time = 0.7f;
    bool iswashing = false;
    WashStage Manager;
    void Start()
    {
        Manager = GameObject.Find("WashStage").GetComponent<WashStage>();
    }

    void Update()
    {
        if (iswashing) time -= Time.deltaTime;

        if (time < 0)
        {
            Manager.CurrentBubbleCount++;
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Trigger") iswashing = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Trigger") iswashing = false;
    }
}
