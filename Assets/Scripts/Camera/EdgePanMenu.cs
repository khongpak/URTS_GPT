using UnityEngine;
using UnityEngine.UI;
using URTS_GPT.CameraSystem; // ใช้ RtsCamera จากเนมสเปซที่คุณตั้ง

namespace URTS_GPT.UI
{
    public class EdgePanMenu : MonoBehaviour
    {
        [SerializeField] private Toggle edgePanToggle;
        [SerializeField] private RtsCamera rtsCamera;

        private const string PrefKey = "EdgePanEnabled";

        void Awake()
        {
            if (rtsCamera == null)
            {
#if UNITY_2023_1_OR_NEWER
                rtsCamera = FindFirstObjectByType<RtsCamera>();
#else
                rtsCamera = FindObjectOfType<RtsCamera>();
#endif
            }
        }

        void Start()
        {
            // โหลดค่าที่เคยบันทึก (ค่าเริ่มต้น: เปิด = 1)
            bool isOn = PlayerPrefs.GetInt(PrefKey, 1) == 1;
            if (edgePanToggle != null) edgePanToggle.isOn = isOn;
            if (rtsCamera != null) rtsCamera.SetEdgePan(isOn);

            if (edgePanToggle != null)
                edgePanToggle.onValueChanged.AddListener(OnToggleChanged);
        }

        void OnDestroy()
        {
            if (edgePanToggle != null)
                edgePanToggle.onValueChanged.RemoveListener(OnToggleChanged);
        }

        private void OnToggleChanged(bool isOn)
        {
            if (rtsCamera != null) rtsCamera.SetEdgePan(isOn);
            PlayerPrefs.SetInt(PrefKey, isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
