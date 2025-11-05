// Assets/Scripts/Stage/Stages/CakeFireStage/CandleFire.cs
using System;
using UnityEngine;

namespace Stage
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class CandleFire : MonoBehaviour
    {
        [Header("Match")]
        [SerializeField] string lighterTag = "LighterFlame";
        [SerializeField] LayerMask acceptLayers = ~0;

        [Header("Visual")]
        [SerializeField] GameObject fireObject;     // 켜고/끄기용 오브젝트
        [SerializeField] ParticleSystem particle;   // 옵션

        [SerializeField] float igniteDistance = 0.12f; // 원하는 근접 거리

        public bool IsLit { get; private set; }
        public event Action<CandleFire> OnIgnited;

        void Reset()
        {
            var col = GetComponent<Collider2D>();
            if (col) col.isTrigger = true;

            if (!fireObject)
            {
                var t = transform.Find("FireFx");
                if (t) fireObject = t.gameObject;
            }
            if (!particle) particle = GetComponentInChildren<ParticleSystem>(true);
        }

        void OnEnable()
        {
            // 레벨 전환 시 재사용되므로 확실히 꺼진 상태로 시작
            IsLit = false;
            Show(false);
        }

        void Show(bool on)
        {
            if (fireObject && fireObject.activeSelf != on) fireObject.SetActive(on);

            if (particle)
            {
                if (on && !particle.isPlaying) particle.Play();
                if (!on && particle.isPlaying)
                    particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        void TryIgnite(Collider2D other, string src)
        {
            if (IsLit || other == null) return;

            // 레이어/태그 필터
            if (((1 << other.gameObject.layer) & acceptLayers) == 0) return;
            if (!string.IsNullOrEmpty(lighterTag) && !other.CompareTag(lighterTag)) return;

            Ignite(src);
        }

        public void Ignite(string src = "Enter")
        {
            if (IsLit) return;

            IsLit = true;
            Show(true);
            Debug.Log($"[CandleFire:{name}] ignited by Lighter ({src})");

            // ☆ 반드시 이벤트 쏜다 → CakeFireStage가 밝기/카운트 갱신
            OnIgnited?.Invoke(this);
        }

        void OnTriggerEnter2D(Collider2D other) => TryIgnite(other, "Enter");
        void OnTriggerStay2D(Collider2D other)  => TryIgnite(other, "Stay");
    }
}
