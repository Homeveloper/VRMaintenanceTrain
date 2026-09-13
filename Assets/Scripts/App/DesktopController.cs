using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VRMaintenanceTrainer
{
    public sealed class DesktopController : MonoBehaviour
    {
        [SerializeField] private Camera view;
        [SerializeField] private PowerLever lever;
        [SerializeField] private ReplacementPart part;
        [SerializeField] private MaintenanceTool tool;
        [SerializeField] private PartSocket correctSocket;
        [SerializeField] private PartSocket wrongSocket;

        private enum HeldObject
        {
            None,
            Part,
            Tool
        }

        private HeldObject _held;
        private Collider _partCollider;
        private Collider _toolCollider;
        private Rigidbody _partBody;
        private Rigidbody _toolBody;

        private Transform _partStartParent;
        private Transform _toolStartParent;
        private Vector3 _partStartPosition;
        private Vector3 _toolStartPosition;
        private Quaternion _partStartRotation;
        private Quaternion _toolStartRotation;
        private bool _partStartKinematic;
        private bool _toolStartKinematic;
        private bool _partInstalled;

        private void Awake()
        {
            _partCollider = part.GetComponent<Collider>();
            _toolCollider = tool.GetComponent<Collider>();
            _partBody = part.GetComponent<Rigidbody>();
            _toolBody = tool.GetComponent<Rigidbody>();

            _partStartParent = part.transform.parent;
            _toolStartParent = tool.transform.parent;
            _partStartPosition = part.transform.localPosition;
            _toolStartPosition = tool.transform.localPosition;
            _partStartRotation = part.transform.localRotation;
            _toolStartRotation = tool.transform.localRotation;
            _partStartKinematic = _partBody.isKinematic;
            _toolStartKinematic = _toolBody.isKinematic;
        }

        private void Update()
        {
            if (view == null || !view.gameObject.activeInHierarchy)
                return;

            DesktopTarget target = LookTarget();
            bool aimingAtZone = target != null &&
                                target.Kind == DesktopTargetKind.WorkZone;

            Mouse mouse = Mouse.current;

            if (mouse != null && mouse.rightButton.wasPressedThisFrame)
                ReturnHeldObject();

            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
                HandleClick(target);

            if (_held == HeldObject.Tool)
            {
                tool.transform.position = view.transform.TransformPoint(
                    new Vector3(0.3f, -0.23f, 0.65f));
                tool.transform.rotation = view.transform.rotation;
            }

            tool.SetDesktopState(
                _held == HeldObject.Tool,
                aimingAtZone,
                mouse != null && mouse.leftButton.isPressed);

            if (Keyboard.current != null &&
                Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                SceneFlow.ReturnToMenu();
            }
        }

        private DesktopTarget LookTarget()
        {
            Vector3 screenPosition = Mouse.current != null
                ? (Vector3)Mouse.current.position.ReadValue()
                : new Vector3(Screen.width / 2f, Screen.height / 2f);

            Ray ray = view.ScreenPointToRay(screenPosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 6f);

            Array.Sort(hits, (a, b) =>
                a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in hits)
            {
                DesktopTarget target =
                    hit.collider.GetComponent<DesktopTarget>();

                if (target != null)
                    return target;
            }

            return null;
        }

        private void HandleClick(DesktopTarget target)
        {
            if (target == null)
                return;

            switch (target.Kind)
            {
                case DesktopTargetKind.Lever:
                    lever.Flip();
                    break;

                case DesktopTargetKind.Part:
                    HoldPart();
                    break;

                case DesktopTargetKind.Tool:
                    HoldTool();
                    break;

                case DesktopTargetKind.CorrectSocket:
                    if (_held == HeldObject.Part)
                        PlacePart(correctSocket);
                    break;

                case DesktopTargetKind.WrongSocket:
                    if (_held == HeldObject.Part)
                        PlacePart(wrongSocket);
                    break;
            }
        }

        private void HoldPart()
        {
            if (_held != HeldObject.None || _partInstalled)
                return;

            _held = HeldObject.Part;
            _partBody.isKinematic = true;
            _partCollider.enabled = false;

            part.transform.SetParent(view.transform);
            part.transform.localPosition =
                new Vector3(-0.3f, -0.22f, 0.7f);
            part.transform.localRotation = Quaternion.identity;
        }

        private void HoldTool()
        {
            if (_held != HeldObject.None)
                return;

            _held = HeldObject.Tool;
            _toolBody.isKinematic = true;
            _toolCollider.enabled = false;
        }

        private void PlacePart(PartSocket socket)
        {
            if (!socket.Install())
                return;

            part.transform.SetParent(null);
            part.transform.position = socket.transform.position;
            part.transform.rotation = socket.transform.rotation;

            _partCollider.enabled = true;
            _partInstalled = true;
            _held = HeldObject.None;
        }

        private void ReturnHeldObject()
        {
            if (_held == HeldObject.Part)
            {
                _partBody.isKinematic = true;
                part.transform.SetParent(_partStartParent, false);
                part.transform.localPosition = _partStartPosition;
                part.transform.localRotation = _partStartRotation;
                _partCollider.enabled = true;
                _partBody.isKinematic = _partStartKinematic;
            }
            else if (_held == HeldObject.Tool)
            {
                _toolBody.isKinematic = true;
                tool.transform.SetParent(_toolStartParent, false);
                tool.transform.localPosition = _toolStartPosition;
                tool.transform.localRotation = _toolStartRotation;
                _toolCollider.enabled = true;
                _toolBody.isKinematic = _toolStartKinematic;
            }

            _held = HeldObject.None;
        }
    }
}