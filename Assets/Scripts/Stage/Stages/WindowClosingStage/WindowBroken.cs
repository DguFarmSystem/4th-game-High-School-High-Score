using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowBroken : MonoBehaviour
{
    Transform parent;
    Collider2D parentCollider;
    MaterialPropertyBlock mpb;
    Bounds bounds;
    SpriteRenderer sr;

    void Start()
    {
        parent = transform.parent;
        parentCollider = parent.GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        mpb = new MaterialPropertyBlock();
        bounds = parentCollider.bounds;

        mpb.SetFloat("_ClipXMin", bounds.min.x);
        mpb.SetFloat("_ClipXMax", bounds.max.x);
        mpb.SetFloat("_ClipYMin", bounds.min.y);
        mpb.SetFloat("_ClipYMax", bounds.max.y);
        
        sr.SetPropertyBlock(mpb);

        GetComponent<SpriteRenderer>().sortingOrder = parentCollider.GetComponent<SpriteRenderer>().sortingOrder;
    }

    void Update()
    {
        bounds = parentCollider.bounds;

        mpb.SetFloat("_ClipXMin", bounds.min.x);
        mpb.SetFloat("_ClipXMax", bounds.max.x);
        mpb.SetFloat("_ClipYMin", bounds.min.y);
        mpb.SetFloat("_ClipYMax", bounds.max.y);

        sr.SetPropertyBlock(mpb);
    }
}
