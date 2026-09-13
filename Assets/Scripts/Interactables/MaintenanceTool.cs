using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace VRMaintenanceTrainer
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public sealed class MaintenanceTool : MonoBehaviour
    {
        [SerializeField] private ScenarioController scenario;
        [SerializeField] private Transform tip;
        [SerializeField] private Transform workZone;
        [SerializeField] private float zoneRadius = 0.22f;
        [SerializeField] private float requiredSeconds = 2f;

        private XRGrabInteractable _grab;
        private bool _xrHeld;
        private bool _xrActivated;
        private bool _desktopHeld;
        private bool _desktopAiming;
        private bool _desktopActivated;
        private float _heldSeconds;
        private float _outOfZoneSeconds;
        private bool _complete;

        public float Progress01 => Mathf.Clamp01(_heldSeconds / requiredSeconds);
        public event Action<float> ProgressChanged;

        private void Awake() => _grab = GetComponent<XRGrabInteractable>();

        private void OnEnable()
        {
            _grab.selectEntered.AddListener(OnGrab);
            _grab.selectExited.AddListener(OnRelease);
            _grab.activated.AddListener(OnActivate);
            _grab.deactivated.AddListener(OnDeactivate);
        }

        private void OnDisable()
        {
            _grab.selectEntered.RemoveListener(OnGrab);
            _grab.selectExited.RemoveListener(OnRelease);
            _grab.activated.RemoveListener(OnActivate);
            _grab.deactivated.RemoveListener(OnDeactivate);
        }

        private void OnGrab(SelectEnterEventArgs _) => _xrHeld = true;
        private void OnRelease(SelectExitEventArgs _) { _xrHeld = false; _xrActivated = false; }
        private void OnActivate(ActivateEventArgs _) => _xrActivated = true;
        private void OnDeactivate(DeactivateEventArgs _) => _xrActivated = false;

        public void SetDesktopState(bool held, bool aiming, bool activated)
        {
            _desktopHeld = held;
            _desktopAiming = aiming;
            _desktopActivated = activated;
        }

        private void Update()
        {
            if (_complete) return;
            var xrInZone = tip != null && workZone != null &&
                           Vector3.Distance(tip.position, workZone.position) <= zoneRadius;
            var active = (_xrHeld && _xrActivated && xrInZone) ||
                         (_desktopHeld && _desktopActivated && _desktopAiming);

            if (active)
            {
                _outOfZoneSeconds = 0f;
                _heldSeconds += Time.deltaTime;
            }
            else if (_heldSeconds > 0f)
            {
                _outOfZoneSeconds += Time.deltaTime;
                if (_outOfZoneSeconds > 0.18f) _heldSeconds = 0f;
            }

            ProgressChanged?.Invoke(Progress01);
            if (_heldSeconds < requiredSeconds) return;

            if (scenario.TryPerform(new ScenarioAction(ScenarioActionType.ToolApplied)))
                _complete = true;
            else
                _heldSeconds = 0f;
        }
    }
}
