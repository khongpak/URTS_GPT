using System.Collections.Generic;
using UnityEngine;
using URTS_GPT.UI;

namespace URTS_GPT.SelectionSystem
{
    public class SelectionManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask selectableMask = ~0; // แนะนำกรองให้เหลือเฉพาะ Layer "Unit"
        [SerializeField] private SelectionBoxUI selectionBoxUI;

        private readonly HashSet<Selectable> selected = new HashSet<Selectable>();
        private Vector2 dragStart;
        private bool dragging;
        private Selectable hovered;

        private void Awake()
        {
            if (mainCamera == null) mainCamera = Camera.main;
        }

        private void Update()
        {
            UpdateHover();
            HandleClickAndDrag();
        }

        private void UpdateHover()
        {
            var hit = Raycast();
            var newHovered = hit.HasValue ? hit.Value.selectable : null;
            if (newHovered != hovered)
            {
                if (hovered != null && !selected.Contains(hovered)) hovered.SetHovered(false);
                hovered = newHovered;
                if (hovered != null && !selected.Contains(hovered)) hovered.SetHovered(true);
            }
        }

        private (Selectable selectable, RaycastHit hit)? Raycast()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 1000f, selectableMask))
            {
                var s = hit.collider.GetComponentInParent<Selectable>();
                if (s != null) return (s, hit);
            }
            return null;
        }

        private void HandleClickAndDrag()
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragStart = Input.mousePosition;
                dragging = true;
                selectionBoxUI?.Begin(dragStart);
            }

            if (dragging && Input.GetMouseButton(0))
            {
                selectionBoxUI?.Drag(Input.mousePosition);
            }

            if (dragging && Input.GetMouseButtonUp(0))
            {
                dragging = false;
                var end = (Vector2)Input.mousePosition;
                selectionBoxUI?.End();

                Rect screenRect = GetScreenRect(dragStart, end);
                bool isBox = screenRect.size.magnitude > 10f; // กันคลิกสั่น

                bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                bool ctrl  = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

                if (!shift && !ctrl) ClearSelection();
                if (isBox) BoxSelect(screenRect, shift, ctrl);
                else ClickSelect(shift, ctrl);
            }
        }

        private void ClickSelect(bool shift, bool ctrl)
        {
            var hit = Raycast();
            if (!hit.HasValue) return;
            var s = hit.Value.selectable;

            if (ctrl)
            {
                if (selected.Contains(s)) Deselect(s);
                else Select(s);
            }
            else
            {
                // shift หรือ none (none ถูกเคลียร์แล้ว)
                Select(s);
            }
        }

        private void BoxSelect(Rect screenRect, bool shift, bool ctrl)
        {
            foreach (var s in Selectable.Registry)
            {
                Vector3 sp = mainCamera.WorldToScreenPoint(s.WorldCenter);
                if (sp.z < 0f) continue; // อยู่หลังกล้อง
                if (screenRect.Contains(sp))
                {
                    if (ctrl && selected.Contains(s)) Deselect(s);
                    else Select(s);
                }
            }
        }

        private void Select(Selectable s)
        {
            if (selected.Add(s))
            {
                s.SetSelected(true);
                s.SetHovered(false);
            }
        }

        private void Deselect(Selectable s)
        {
            if (selected.Remove(s))
            {
                s.SetSelected(false);
                if (s == hovered) s.SetHovered(true); // กลับไปโชว์ hover ถ้ายังอยู่ใต้เมาส์
            }
        }

        public void ClearSelection()
        {
            foreach (var s in selected) s.SetSelected(false);
            selected.Clear();
        }

        private static Rect GetScreenRect(Vector2 start, Vector2 end)
        {
            Vector2 min = Vector2.Min(start, end);
            Vector2 max = Vector2.Max(start, end);
            return new Rect(min, max - min);
        }
    }
}