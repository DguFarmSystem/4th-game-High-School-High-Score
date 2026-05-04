using UnityEngine;

namespace Stage
{
    [DisallowMultipleComponent]
    public class VisionAnswerSelector : MonoBehaviour
    {
        [Header("Icons")]
        [SerializeField] private GameObject[] icons;

        private int currentIndex = 0;
        private bool canTouch = true;

        private void Awake()
        {
            ApplyVisual();
        }

        private void OnMouseDown()
        {
            if (!canTouch) return;
            Next();
        }

        public void Initialize()
        {
            currentIndex = 0;
            canTouch = true;
            ApplyVisual();
        }

        public void SetTouchEnabled(bool enabled)
        {
            canTouch = enabled;
        }

        public int GetCurrentIndex()
        {
            return currentIndex;
        }

        private void Next()
        {
            currentIndex++;

            if (currentIndex >= icons.Length)
                currentIndex = 0;

            ApplyVisual();

            Debug.Log($"[VisionAnswerSelector] currentIndex = {currentIndex}");
        }

        private void ApplyVisual()
        {
            if (icons == null) return;

            for (int i = 0; i < icons.Length; i++)
            {
                if (icons[i] != null)
                    icons[i].SetActive(i == currentIndex);
            }
        }
    }
}