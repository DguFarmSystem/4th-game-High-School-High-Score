using UnityEngine;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class Candle : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] GameObject fireFx;        // 불 이펙트 (켜고/끄기)
    [SerializeField] Animator animator;        // 선택(없어도 됨)
    [SerializeField] string igniteTrigger = "Play"; // 선택
    [SerializeField] string stopTrigger   = "Stop"; // 선택

    public bool IsLit { get; private set; }
    public event Action<Candle> OnIgnited;

    void Reset() {
        // 가장 흔한 경로 자동 탐색
        if (!fireFx) {
            var t = transform.Find("Flame/FireFx") ?? transform.Find("FireFx") ?? transform.Find("Flame/idlefire1");
            if (t) fireFx = t.gameObject;
        }
        if (!animator) animator = GetComponentInChildren<Animator>(true);
    }

    void OnEnable() {
        Show(false, instant:true);
        IsLit = false;
    }

    public void Ignite() {
        if (IsLit) return;
        IsLit = true;
        Show(true, instant:false);
        OnIgnited?.Invoke(this);
    }

    public void ForceOff() {
        IsLit = false;
        Show(false, instant:true);
    }

    void Show(bool on, bool instant) {
        if (fireFx && fireFx.activeSelf != on) fireFx.SetActive(on);
        if (animator) {
            if (on && !string.IsNullOrEmpty(igniteTrigger)) animator.SetTrigger(igniteTrigger);
            if (!on && !string.IsNullOrEmpty(stopTrigger))   animator.SetTrigger(stopTrigger);
        }
    }
}
