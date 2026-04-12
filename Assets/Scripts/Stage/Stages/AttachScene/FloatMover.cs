using UnityEngine;

public class FloatMover : MonoBehaviour
{
    [Header("위아래 이동 거리")]
    [SerializeField] private float moveRange = 0.15f;

    [Header("이동 속도")]
    [SerializeField] private float moveSpeed = 1.0f;

    private Vector3 _startPosition;
    private bool _isStopped = false;

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        if (_isStopped) return;

        float offsetY = Mathf.Sin(Time.time * moveSpeed) * moveRange;
        transform.position = new Vector3(
            _startPosition.x,
            _startPosition.y + offsetY,
            _startPosition.z
        );
    }

    public void StopMoving()
    {
        _isStopped = true;
    }
}