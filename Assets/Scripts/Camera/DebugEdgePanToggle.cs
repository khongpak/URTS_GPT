using UnityEngine;

namespace URTS_GPT.CameraSystem
{
    public class DebugEdgePanToggle : MonoBehaviour
    {
        [SerializeField] RtsCamera rtsCamera;

        void Awake()
        {
            if (rtsCamera == null)
            {
        #if UNITY_2023_1_OR_NEWER
                rtsCamera = FindFirstObjectByType<RtsCamera>(); // ใหม่
        #else
                rtsCamera = FindObjectOfType<RtsCamera>();      // เก่า
        #endif
            }
        }

    #if UNITY_EDITOR
        void OnGUI()
        {
            if (rtsCamera == null) return;

            var rect = new Rect(10, 10, 160, 24);
            bool isOn = GUI.Toggle(rect, rtsCamera.IsEdgePanEnabled, " Edge Pan");
            if (isOn != rtsCamera.IsEdgePanEnabled)
                rtsCamera.SetEdgePan(isOn);
        }
    #endif
    }
}