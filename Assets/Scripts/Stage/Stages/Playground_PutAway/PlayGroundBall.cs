using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class PlayGroundBall : MonoBehaviour
{
    [SerializeField] private bool isRealBall;

    private bool isInBasket = false;

    public bool IsRealBall() => isRealBall;
    public bool IsInBasket() => isInBasket;
    public void SetInBasket(bool value) => isInBasket = value;

    public bool IsFullyInside(Collider2D target)
    {
        /*
        if (a is PolygonCollider2D poly)
            return PolygonInside(poly, b);
        

        if (a is BoxCollider2D box)
            return IsBoxInside(box, b);

        if (a is CircleCollider2D circle)
            return IsCircleInside(circle, b);
        */

        // lossyScale: 부모 객체를 고려한 객체의 절대 크기를 반환 
        Vector2 center = GetComponent<CircleCollider2D>().transform.TransformPoint(GetComponent<CircleCollider2D>().offset);
        float radius = GetComponent<CircleCollider2D>().radius * GetComponent<CircleCollider2D>().transform.lossyScale.x;
    
        int sample = 16; // 샘플링 포인트 수

        for (int i = 0; i < sample; i++)
        {
            float angle = i * Mathf.PI * 2 / sample;

            Vector2 point = center + new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * radius;

            if (!target.OverlapPoint(point)) return false;
        }

        return true;
    }

    /*
    bool IsPolygonInside(Collider2D a, Collider2D b)
    {
        PolygonCollider2D poly = a as PolygonCollider2D;

        foreach (Vector2 point in poly.points)
        {
            Vector2 worldPoint = poly.transform.TransformPoint(point);

            if (!b.OverlapPoint(worldPoint))
                return false;
        }

        return true;
    }

    bool IsBoxInside(BoxCollider2D box, Collider2D target)
    {
        Vector2 size = Vector2.Scale(box.size, box.transform.lossyScale) * 0.5f;

        Vector2[] localPoints =
        {
            new Vector2(-size.x, -size.y),
            new Vector2(size.x, -size.y),
            new Vector2(size.x, size.y),
            new Vector2(-size.x, size.y)
        };

        foreach (var p in localPoints)
        {
            Vector2 world = box.transform.TransformPoint(p + box.offset);

            if (!target.OverlapPoint(world))
                return false;
        }

        return true;
    }

    bool IsCircleInside(CircleCollider2D circle, Collider2D target)
    {
        Vector2 center = circle.transform.TransformPoint(circle.offset);
        float radius = circle.radius * circle.transform.lossyScale.x;

        int sample = 16; // 샘플링 포인트 수

        for (int i = 0; i < sample; i++)
        {
            float angle = i * Mathf.PI * 2 / sample;

            Vector2 point = center + new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * radius;

            if (!target.OverlapPoint(point))
                return false;
        }

        return true;
    }
    */
}
