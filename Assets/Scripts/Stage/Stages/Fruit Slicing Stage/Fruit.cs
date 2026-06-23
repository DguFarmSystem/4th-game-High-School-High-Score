using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{

    Rigidbody2D rigid;

    [SerializeField]
    GameObject[] Debris;

    GameObject a;
    GameObject b;

    float DestroyTime = 1f;
    bool Sliced;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        if (transform.position.x < 0)
        {
            rigid.velocity = new Vector2(10, 7);
            rigid.angularVelocity = Random.Range(-100, 100);
        }
        else
        {
            rigid.velocity = new Vector2(-10, 7);
            rigid.angularVelocity = Random.Range(-100, 100);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.tag == "Untagged")
        {
            a=Instantiate(Debris[0],transform.position,transform.rotation);
            b = Instantiate(Debris[1], transform.position, transform.rotation);
            a.GetComponent<Rigidbody2D>().velocity= rigid.velocity;
            a.GetComponent<Rigidbody2D>().angularVelocity= rigid.angularVelocity;
            b.GetComponent<Rigidbody2D>().velocity = rigid.velocity;
            b.GetComponent<Rigidbody2D>().angularVelocity = rigid.angularVelocity;
            Vector2 TheVector = transform.right;
            a.GetComponent<Rigidbody2D>().velocity += TheVector * -3;
            b.GetComponent<Rigidbody2D>().velocity += TheVector * 3;
            GetComponent<SpriteRenderer>().enabled = false;
            Sliced = true;
        }
        if (Sliced)
        {
            DestroyTime -= Time.deltaTime;
            if (DestroyTime < 0.9f)
            {
                if (gameObject.name == "Slice_Banana(Clone)")
                    GetComponent<BoxCollider2D>().enabled = false;
                else
                    GetComponent<CircleCollider2D>().enabled = false;
            }
            if (DestroyTime < 0) Destroy(gameObject);
        }
    }
}
