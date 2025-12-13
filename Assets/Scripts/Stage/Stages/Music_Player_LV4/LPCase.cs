using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LPCase : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _lpCaseIndicator;
    [SerializeField] private Vector3 _lpCaseTargetPos;

    private Music_Player_LV4 _stage;

    public void OnTap(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.TouchedCollider == this.GetComponent<Collider2D>())
        {
            _stage.ClearConditionsQ.Dequeue();
            StartCoroutine(DisappearLPCase());

            InputManager.Instance._tapAction.performed -= OnTap;
        }
    }

    IEnumerator DisappearLPCase()
    {
        _lpCaseIndicator.enabled = false;

        float elapsedTime = 0f;
        Vector3 startingPos = transform.position;

        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.5f;
            t = Mathf.SmoothStep(0f, 1f, t); // 0→1 사이를 부드럽게 증가

            transform.position = Vector3.Lerp(startingPos, _lpCaseTargetPos, t);

            yield return null; // 다음 프레임까지 대기
        }
    }

    void Start()
    {
        _stage = FindObjectOfType<Music_Player_LV4>();

        if (InputManager.Instance != null)
        {
            InputManager.Instance._tapAction.performed += OnTap;
        }
    }

    void OnDisable()
    {
        if (InputManager.Instance != null && InputManager.Instance._tapAction != null)
        {
            InputManager.Instance._tapAction.performed -= OnTap;
        }
    }
}
