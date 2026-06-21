using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weight : MonoBehaviour
{

    public AudioClip Effect;
    AudioSource theaudio;

    public bool IsInRange=false;

    public int w;

    public bool set = false;

    void Start()
    {
        theaudio=GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!set&&(transform.position.x<1.79||transform.position.x> 8.52f||transform.position.y< 1.42f) && GetComponent<BoxCollider2D>().enabled) Destroy(this.gameObject);
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
            if (!set)
            {
                transform.position = new Vector3(5.3f, transform.position.y, transform.position.z);
                theaudio.Play();
            }
            set= true;
            if (collision.gameObject.GetComponent<Weight>().w < GetComponent<Weight>().w)
            {
                Vector3 temp = collision.transform.position;
                collision.transform.position = transform.position;
                transform.position = temp;
            }
        }
    }

}
