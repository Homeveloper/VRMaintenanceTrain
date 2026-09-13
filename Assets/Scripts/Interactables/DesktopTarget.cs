using UnityEngine;

namespace VRMaintenanceTrainer
{
    public enum DesktopTargetKind { Lever, Part, CorrectSocket, WrongSocket, Tool, WorkZone }

    public sealed class DesktopTarget : MonoBehaviour
    {
        [SerializeField] private DesktopTargetKind kind;
        public DesktopTargetKind Kind => kind;
    }
}
