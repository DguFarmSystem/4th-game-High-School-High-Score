using UnityEngine;

public class MagnetTriggerRelay : MonoBehaviour
{
    [SerializeField] private MagnetController magnetController;

    private void Awake()
    {
        if (magnetController == null)
            magnetController = GetComponentInParent<MagnetController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        AttachableObject target = other.GetComponent<AttachableObject>();
        if (target == null)
            target = other.GetComponentInParent<AttachableObject>();

        if (target == null)
        {
            Debug.Log("[MagnetTriggerRelay] 무시됨(붙일 수 없는 오브젝트): " + other.name);
            return;
        }

        Debug.Log("[MagnetTriggerRelay] Trigger Enter: " + other.name);

        if (magnetController != null)
            magnetController.TryAttach(other);
        else
            Debug.LogWarning("[MagnetTriggerRelay] magnetController가 비어 있음");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other == null) return;

        AttachableObject target = other.GetComponent<AttachableObject>();
        if (target == null)
            target = other.GetComponentInParent<AttachableObject>();

        if (target != null)
            Debug.Log("[Stay] " + other.name);
    }
}