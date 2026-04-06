using UnityEngine;
using Stage;

public class MagnetController : MonoBehaviour
{
    [Header("자석 슬롯들")]
    [SerializeField] private Transform[] attachSlots;

    private bool[] _slotOccupied;
    private AttachStage _stage;

    public void Initialize(AttachStage stage)
    {
        _stage = stage;

        if (attachSlots == null)
            attachSlots = new Transform[0];

        _slotOccupied = new bool[attachSlots.Length];

        for (int i = 0; i < _slotOccupied.Length; i++)
            _slotOccupied[i] = false;
    }

    public void TryAttach(Collider2D other)
    {
        if (other == null) return;

        // 1) 자기 자신 / 자식 / 부모 쪽 콜라이더는 무시
        if (other.transform.IsChildOf(transform) || transform.IsChildOf(other.transform))
        {
            Debug.Log("[MagnetController] 자기 자신 충돌 무시: " + other.name);
            return;
        }

        Debug.Log("[MagnetController] TryAttach: " + other.name);

        AttachableObject target = other.GetComponent<AttachableObject>();
        if (target == null)
            target = other.GetComponentInParent<AttachableObject>();

        if (target == null)
        {
            Debug.LogWarning("[MagnetController] AttachableObject 못 찾음");
            return;
        }

        Debug.Log("[MagnetController] target = " + target.name);

        if (target.IsAttached)
        {
            Debug.LogWarning("[MagnetController] 이미 붙어 있음");
            return;
        }

        int slotIndex = target.PreferredSlotIndex;
        Debug.Log("[MagnetController] slotIndex = " + slotIndex);

        if (!IsValidSlot(slotIndex))
        {
            Debug.LogWarning("[MagnetController] 유효하지 않은 슬롯 인덱스");
            return;
        }

        if (_slotOccupied[slotIndex])
        {
            Debug.LogWarning("[MagnetController] 슬롯이 이미 사용 중");
            return;
        }

        Transform slot = attachSlots[slotIndex];
        if (slot == null)
        {
            Debug.LogWarning("[MagnetController] 슬롯 Transform이 null");
            return;
        }

        bool attached = target.AttachTo(slot);
        Debug.Log("[MagnetController] Attach 결과 = " + attached);

        if (!attached) return;

        _slotOccupied[slotIndex] = true;

        if (_stage != null)
        {
            _stage.PlayAttachSfx();
            _stage.NotifyObjectAttached(target);
        }
    }

    private bool IsValidSlot(int index)
    {
        return attachSlots != null && index >= 0 && index < attachSlots.Length;
    }
}