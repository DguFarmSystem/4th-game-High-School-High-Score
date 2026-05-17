using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class DragMagnet : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform followRoot;
    [SerializeField] private bool returnToStartOnRelease = false;

    [Header("Bounds")]
    [SerializeField] private bool clampToBounds = true;
    [SerializeField] private Vector2 minBounds = new Vector2(-2.2f, -3.8f);
    [SerializeField] private Vector2 maxBounds = new Vector2(2.2f, 3.8f);

    [Header("Pick")]
    [SerializeField] private bool clickOnlyOnMagnet = true;
    [SerializeField] private LayerMask pickableLayers = ~0;

    private Vector3 _startPos;
    private bool _holding;

    private readonly List<Collider2D> _myCols = new List<Collider2D>(8);
    private static readonly Collider2D[] _overlapBuf = new Collider2D[16];

    

    private void Awake()
    {
        if (!followRoot) followRoot = transform;
        _startPos = followRoot.position;

        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
        rb.useFullKinematicContacts = true;
        rb.gravityScale = 0f;

        _myCols.Clear();
        GetComponentsInChildren(true, _myCols);
    }

    private void Update()
    {
        bool down = false, hold = false, up = false;

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            down = t.phase == TouchPhase.Began;
            hold = t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary;
            up   = t.phase == TouchPhase.Canceled || t.phase == TouchPhase.Ended;
        }
        else
        {
            down = Input.GetMouseButtonDown(0);
            hold = Input.GetMouseButton(0);
            up   = Input.GetMouseButtonUp(0);
        }

        if (down)
        {
            if (!clickOnlyOnMagnet)
            {
                _holding = true;
            }
            else
            {
                Vector2 wp = PointerWorldXY();
                int n = Physics2D.OverlapPointNonAlloc(wp, _overlapBuf, pickableLayers);

                bool onMe = false;
                for (int i = 0; i < n; i++)
                {
                    if (_myCols.Contains(_overlapBuf[i]))
                    {
                        onMe = true;
                        break;
                    }
                }

                _holding = onMe;
            }
        }

        if (_holding && hold)
        {
            Vector2 p = PointerWorldXY();
            Vector3 nextPos = new Vector3(p.x, p.y, followRoot.position.z);

            if (clampToBounds)
            {
                nextPos.x = Mathf.Clamp(nextPos.x, minBounds.x, maxBounds.x);
                nextPos.y = Mathf.Clamp(nextPos.y, minBounds.y, maxBounds.y);
            }

            followRoot.position = nextPos;
        }

        if (up)
        {
            _holding = false;

            if (returnToStartOnRelease)
                followRoot.position = _startPos;
        }
    }

    private Vector2 PointerWorldXY()
    {
        Camera cam = Camera.main;
        Vector3 sp = (Input.touchCount > 0)
            ? (Vector3)Input.GetTouch(0).position
            : Input.mousePosition;

        if (!cam || cam.orthographic)
        {
            Vector3 v = cam ? cam.ScreenToWorldPoint(sp) : sp;
            return new Vector2(v.x, v.y);
        }
        else
        {
            float z = Mathf.Abs(followRoot.position.z - cam.transform.position.z);
            Vector3 v = cam.ScreenToWorldPoint(new Vector3(sp.x, sp.y, z));
            return new Vector2(v.x, v.y);
        }
    }
}