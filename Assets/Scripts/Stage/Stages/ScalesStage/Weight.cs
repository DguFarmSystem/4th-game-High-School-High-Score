using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weight : MonoBehaviour
{

    public AudioClip Effect;
    AudioSource theaudio;

    public bool IsInRange=false;

    void Start()
    {
        theaudio=GetComponent<AudioSource>();
    }

    void Update()
    {
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.transform.position.y<transform.position.y)
        {
            collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1 ;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.transform.position.y < transform.position.y && IsInRange)
        {
            theaudio.Play();
        }
    }

}
