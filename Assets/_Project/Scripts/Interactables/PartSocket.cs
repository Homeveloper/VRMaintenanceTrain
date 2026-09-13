using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace VRMaintenanceTrainer
{
    [RequireComponent(typeof(XRSocketInteractor))]
    public sealed class PartSocket : MonoBehaviour, IXRSelectFilter
    {
        [SerializeField] private ScenarioController scenario;
        [SerializeField] private string socketId;

        private XRSocketInteractor _socket;
        private bool _registered;
        private bool _blockedUntilExit;

        private bool IsCorrectStep => scenario.StepIndex == 1 && socketId == "replacement";

        public bool canProcess => isActiveAndEnabled;

        private void Awake() => _socket = GetComponent<XRSocketInteractor>();

        private void OnEnable()
        {
            _socket.selectFilters.Add(this);
            _socket.selectEntered.AddListener(OnSelect);
        }

        private void OnDisable()
        {
            _socket.selectEntered.RemoveListener(OnSelect);
            _socket.selectFilters.Remove(this);
        }

        public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
        {
            if (interactable.transform.GetComponent<ReplacementPart>() == null)
                return false;

            return _registered || (IsCorrectStep && !_blockedUntilExit);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<ReplacementPart>() == null || _registered)
                return;

            if (!IsCorrectStep)
            {
                _blockedUntilExit = true;
                scenario.TryPerform(new ScenarioAction(ScenarioActionType.PartInstalled, socketId));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<ReplacementPart>() != null)
                _blockedUntilExit = false;
        }

        private void OnSelect(SelectEnterEventArgs args)
        {
            if (args.interactableObject.transform.GetComponent<ReplacementPart>() != null)
                Install();
        }

        public bool Install()
        {
            if (_registered)
                return false;

            _registered = scenario.TryPerform(
                new ScenarioAction(ScenarioActionType.PartInstalled, socketId));

            return _registered;
        }
    }
}
