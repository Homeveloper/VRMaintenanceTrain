using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace VRMaintenanceTrainer
{
    [RequireComponent(typeof(XRSimpleInteractable))]
    public sealed class PowerLever : MonoBehaviour
    {
        [SerializeField] private ScenarioController scenario;
        [SerializeField] private Transform handle;
        private XRSimpleInteractable _interactable;
        private bool _used;

        private void Awake() => _interactable = GetComponent<XRSimpleInteractable>();
        private void OnEnable() => _interactable.selectEntered.AddListener(OnSelect);
        private void OnDisable() => _interactable.selectEntered.RemoveListener(OnSelect);
        private void OnSelect(SelectEnterEventArgs _) => Flip();

        public void Flip()
        {
            if (_used || !scenario.TryPerform(new ScenarioAction(ScenarioActionType.PowerOff))) return;
            _used = true;
            _interactable.enabled = false;
            if (handle != null) StartCoroutine(AnimateHandle());
        }

        private IEnumerator AnimateHandle()
        {
            var start = handle.localRotation;
            var end = start * Quaternion.Euler(75f, 0f, 0f);
            for (var t = 0f; t < 0.3f; t += Time.deltaTime)
            {
                handle.localRotation = Quaternion.Slerp(start, end, t / 0.3f);
                yield return null;
            }
            handle.localRotation = end;
        }
    }
}
