// Assets/Scripts/Stage/Stages/CakeFireStage/Lighter.cs
using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class Lighter : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform followRoot;            // 비우면 본체
    [SerializeField] Collider2D flameTipTrigger;      // 자식 FlameTip(Trigger)

    [Header("Behavior")]
    [SerializeField] bool returnToStartOnRelease = false;
    [Tooltip("라이터 위를 눌렀을 때만 집히도록 할지")]
    [SerializeField] bool clickOnlyOnLighter = true;
    [SerializeField] LayerMask pickableLayers = ~0;   // 클릭 검사 레이어

    Vector3 _startPos;
    bool _holding;

    // 내가 가진 모든 자식 콜라이더(라이터 몸통 포함)
    readonly List<Collider2D> _myCols = new List<Collider2D>(8);

    void Awake()
    {
        if (!followRoot) followRoot = transform;
        _startPos = followRoot.position;

        // Kinematic RB2D 권장 세팅
        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
        rb.useFullKinematicContacts = true;

        // 내/자식 콜라이더 캐시
        GetComponentsInChildren(true, _myCols);

        // FlameTip 자동 세팅 + Tag/Trigger 보정
        if (!flameTipTrigger)
        {
            var t = transform.Find("FlameTip");
            if (t) flameTipTrigger = t.GetComponent<Collider2D>();
        }
        if (flameTipTrigger)
        {
            flameTipTrigger.isTrigger = true;
            // 태그는 프로젝트에 "LighterFlame" 만들어두고 필요 시 수동 설정
            if (flameTipTrigger.gameObject.tag == "Untagged")
                flameTipTrigger.gameObject.tag = "LighterFlame";
        }
    }

    void Update()
    {
        // 포인터 상태 읽기
        bool down = false, hold = false, up = false;
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
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

        // Down: 클릭 조건 검사
        if (down)
        {
            if (!clickOnlyOnLighter)
            {
                _holding = true;
            }
            else
            {
                // 포인터 월드 좌표 → 해당 위치의 2D 콜라이더들 중 내 콜라이더가 있는지 검사
                Vector2 wp = PointerWorldXY();
                int n = Physics2D.OverlapPointNonAlloc(wp, _overlapBuf, pickableLayers);
                bool onMe = false;
                for (int i = 0; i < n; i++)
                {
                    if (_myCols.Contains(_overlapBuf[i])) { onMe = true; break; }
                }
                _holding = onMe;
            }
        }

        // Hold: 따라가기
        if (_holding && hold)
        {
            var p = PointerWorldXY();
            followRoot.position = new Vector3(p.x, p.y, followRoot.position.z); // z 고정
        }

        // Up: 놓기
        if (up)
        {
            _holding = false;
            if (returnToStartOnRelease) followRoot.position = _startPos;
        }
    }

    // --------- Helpers ---------
    static readonly Collider2D[] _overlapBuf = new Collider2D[16];

    // 화면 포인터 → 월드 XY (정사영/원근 카메라 모두 대응)
    Vector2 PointerWorldXY()
    {
        var cam = Camera.main;
        Vector3 sp = (Input.touchCount > 0) ? (Vector3)Input.GetTouch(0).position : Input.mousePosition;

        if (!cam || cam.orthographic)
        {
            var v = cam ? cam.ScreenToWorldPoint(sp) : sp;
            return new Vector2(v.x, v.y);
        }
        else
        {
            // 원근이라면 라이터 Z평면으로 투영
            float z = Mathf.Abs(followRoot.position.z - cam.transform.position.z);
            var v = cam.ScreenToWorldPoint(new Vector3(sp.x, sp.y, z));
            return new Vector2(v.x, v.y);
        }
    }

    // 마우스 전용 콜백도 같이 둬서 에디터 테스트 안정화
    void OnMouseDown()  { if (clickOnlyOnLighter) _holding = true; }
    void OnMouseUp()    { if (clickOnlyOnLighter) _holding = false; }
}
