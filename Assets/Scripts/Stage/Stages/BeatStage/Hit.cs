using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour
{
    private bool _triggered = false;
    private bool useInputManager = true;

    private BeatStageManager _beatStage;
    private Collider2D _collider2D;

    void Awake()
    {
        _beatStage = FindObjectOfType<BeatStageManager>();
        _collider2D = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        if (useInputManager && InputManager.Instance != null)
        {
            // ensure no duplicate
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

    // Editor / Mouse support
    private void OnMouseDown()
    {
        RegisterHit();
    }

    // Called by InputManager subscription when a tap happens
    private void OnStageTap()
    {
        if (_collider2D == null || InputManager.Instance == null) return;

        // Prefer InputManager's tapped collider if available
        Collider2D tapped = InputManager.Instance.TappedCollider;
        if (tapped != null)
        {
            if (tapped == _collider2D)
            {
                RegisterHit();
            }
            return;
        }

        // Fallback: check TouchWorldPos overlap
        Vector3 worldPos = InputManager.Instance.TouchWorldPos;
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);
        if (hits != null && System.Array.Exists(hits, c => c == _collider2D))
        {
            RegisterHit();
            return;
        }
    }

    private void RegisterHit()
    {
        if (_triggered) return;
        _triggered = true;
        Debug.Log("YYY");
        if (_beatStage != null)
        {
            _beatStage.rightcount++;
        }
    }
}
