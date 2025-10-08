using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCommand : MonoBehaviour
{
    private Vector3 _outPosition;
    private Vector3 _showPosition;
    public IEnumerator ShowAndOut()
    {
        transform.position = _outPosition;

        float elapsedTime = 0;

        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.5f;

            transform.position = Vector3.Lerp(_outPosition, _showPosition, t);

            yield return null; // 다음 프레임까지 대기
        }

        elapsedTime = 0f;

        yield return new WaitForSeconds(1.3f);

        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.5f;

            transform.position = Vector3.Lerp(_showPosition, _outPosition, t);

            yield return null; // 다음 프레임까지 대기
        }
    }

    // ========== Lifecycle methods ========== //
    void Awake()
    {
        _outPosition = transform.position + Vector3.up * 700f;
        _showPosition = transform.position;
    }

    void Start()
    {
        StartCoroutine(ShowAndOut());
    }
}
