using UnityEngine;

[DisallowMultipleComponent]
public class AttachableObject : MonoBehaviour
{
    [Header("물체가 붙을 자석 슬롯 번호")]
    [SerializeField] private int preferredSlotIndex = 0;

    [Header("물체 내부에서 자석에 닿는 기준점")]
    [SerializeField] private Transform attachPivot;

    [Header("붙은 뒤 로컬 회전값")]
    [SerializeField] private Vector3 attachedLocalEulerAngles = Vector3.zero;

    [Header("붙은 뒤 콜라이더 비활성화")]
    [SerializeField] private bool disableColliderOnAttach = true;

    private bool _isAttached = false;

    public bool IsAttached => _isAttached;
    public int PreferredSlotIndex => preferredSlotIndex;

    public bool AttachTo(Transform targetSlot) {
        if (_isAttached) return false;
        if (targetSlot == null) return false;

        _isAttached = true;

        StopMovement();

        Quaternion worldRot = transform.rotation;

        if (attachPivot != null)
        {
            Vector3 offset = transform.position - attachPivot.position;
            transform.position = targetSlot.position + offset;
        }
        else
        {
            transform.position = targetSlot.position;
        }

        transform.SetParent(targetSlot, true);
        transform.rotation = worldRot;   // 현재 월드 회전 유지

        if (disableColliderOnAttach)
        {
            Collider2D[] cols = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D col in cols)
                col.enabled = false;
        }

        Debug.Log("[AttachableObject] Attach 완료: " + name);
        return true;
    }

    private void StopMovement()
    {
        FloatMover floatMover = GetComponent<FloatMover>();
        if (floatMover != null)
            floatMover.StopMoving();

        FishMover fishMover = GetComponent<FishMover>();
        if (fishMover != null)
            fishMover.StopMoving();
    }
}