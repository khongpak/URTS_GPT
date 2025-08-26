using UnityEngine;
using UnityEngine.UI;

namespace URTS_GPT.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SelectionBoxUI : MonoBehaviour
    {
        [SerializeField] private Image image;
        private RectTransform rect;
        private Vector2 start;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            if (image == null) image = GetComponent<Image>();
            Hide();
        }

        public void Begin(Vector2 screenStart)
        {
            start = screenStart;
            gameObject.SetActive(true);
            SetRect(start, start);
        }

        public void Drag(Vector2 screenNow) => SetRect(start, screenNow);
        public void End() => Hide();

        private void SetRect(Vector2 a, Vector2 b)
        {
            Vector2 min = Vector2.Min(a, b);
            Vector2 max = Vector2.Max(a, b);
            rect.anchoredPosition = min;
            rect.sizeDelta = max - min;
            image.enabled = rect.sizeDelta.sqrMagnitude > 1f;
        }

        private void Hide()
        {
            image.enabled = false; // คง object ไว้เพื่อไม่ให้ GC บ่อย
            rect.sizeDelta = Vector2.zero;
        }
    }
}