using UnityEngine;

namespace URTS_GPT.Camera
{
    public class DebugEdgePanToggle : MonoBehaviour
    {
        [SerializeField] RtsCamera rtsCamera;

        void Awake()
        {
            if (rtsCamera == null) rtsCamera = FindFirstObjectByType<RtsCamera>();
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