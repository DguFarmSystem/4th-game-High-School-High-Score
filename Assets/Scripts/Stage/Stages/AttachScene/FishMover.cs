using UnityEngine;

public class FishMover : MonoBehaviour
{
    [Header("왼쪽(-1) / 오른쪽(1)")]
    [SerializeField] private float direction = 1f;

    [Header("이동 속도")]
    [SerializeField] private float moveSpeed = 0.9f;

    private bool _isStopped = false;

    private void Update()
    {
        if (_isStopped) return;

        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime, Space.World);
    }

    public void StopMoving()
    {
        _isStopped = true;
    }
}