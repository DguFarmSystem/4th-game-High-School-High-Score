using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Count : MonoBehaviour
{
    private bool useInputManager = true;

    private Collider2D _collider2D;
    private bool _triggered = false;

    void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        if (useInputManager && InputManager.Instance != null)
        {
            InputManager.Instance.OnStageTapPerformed -= OnStageTap;
            InputManager.Instance.OnStageTapPerformed += OnStageTap;
        }
    }

    private void OnDisable()
    {
        if (useInputManager && InputManager.Instance != null)
        {
            InputManager.Instance.OnStageTapPerformed -= OnStageTap;
        }
    }

    private void OnMouseDown()
    {
        TriggerFailure();
        Debug.Log("잘못 클릭하여 실패");
    }

    private void OnStageTap()
    {
        if (_triggered) return;
        if (_collider2D == null || InputManager.Instance == null) return;

        Collider2D tapped = InputManager.Instance.TappedCollider;
        if (tapped != null)
        {
            if (tapped == _collider2D)
            {
                TriggerFailure();
            }
            return;
        }

        Vector3 worldPos = InputManager.Instance.TouchWorldPos;
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);
        if (hits != null && System.Array.Exists(hits, c => c == _collider2D))
        {
            TriggerFailure();
            return;
        }
    }

    private void TriggerFailure()
    {
        if (_triggered) return;
        _triggered = true;

        // BeatStageManager의 wrong을 true로 설정
        var beatStage = FindObjectOfType<BeatStageManager>();
        if (beatStage != null)
        {
            var wrongField = beatStage.GetType().GetField("wrong", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (wrongField != null)
            {
                wrongField.SetValue(beatStage, true);
            }
        }
    }
}
