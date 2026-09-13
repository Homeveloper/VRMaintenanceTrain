using UnityEngine;
using UnityEngine.XR;

namespace VRMaintenanceTrainer
{
    public sealed class RuntimeMode : MonoBehaviour
    {
        [SerializeField] private GameObject xrRig;
        [SerializeField] private Camera desktopCamera;
        [SerializeField] private bool simulateXRInEditor;

        private void Start()
        {
            bool useXR = XRSettings.isDeviceActive;

#if UNITY_EDITOR
            useXR |= simulateXRInEditor;
#endif

            if (xrRig != null)
                xrRig.SetActive(useXR);

            if (desktopCamera != null)
                desktopCamera.gameObject.SetActive(!useXR);
        }
    }
}