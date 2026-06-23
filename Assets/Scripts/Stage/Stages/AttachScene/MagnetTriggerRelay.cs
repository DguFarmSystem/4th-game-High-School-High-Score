using UnityEngine;

public class MagnetTriggerRelay : MonoBehaviour
{
    [SerializeField] private MagnetController magnetController;
    [SerializeField] private Collider2D triggerCollider;

    private readonly Collider2D[] _results = new Collider2D[20];

    private void Awake()
    {
        if (magnetController == null)
            magnetController = GetComponentInParent<MagnetController>();

        if (triggerCollider == null)
            triggerCollider = GetComponent<Collider2D>();

        if (triggerCollider != null)
            triggerCollider.isTrigger = true;
    }

    private void Update()
    {
        if (magnetController == null)
        {
            Debug.LogWarning("[MagnetTriggerRelay] magnetController 없음");
            return;
        }

        if (triggerCollider == null)
        {
            Debug.LogWarning("[MagnetTriggerRelay] triggerCollider 없음");
            return;
        }

        int count = triggerCollider.OverlapCollider(new ContactFilter2D().NoFilter(), _results);

        for (int i = 0; i < count; i++)
        {
            Collider2D other = _results[i];
            if (other == null) continue;

            Transform magnetTransform = magnetController.transform;

            // 자기 자신 / 자식 콜라이더 무시
            if (other.transform == magnetTransform ||
                other.transform.IsChildOf(magnetTransform))
            {
                continue;
            }

            AttachableObject target = other.GetComponent<AttachableObject>();

            if (target == null)
                target = other.GetComponentInParent<AttachableObject>();

            if (target == null) continue;
            if (target.IsAttached) continue;

            Debug.Log("[MagnetTriggerRelay] 겹침 감지됨: " + target.name);

            magnetController.TryAttach(other);
        }
    }
}