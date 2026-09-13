using UnityEngine;

namespace VRMaintenanceTrainer
{
    public sealed class MenuScreen : MonoBehaviour
    {
        public void StartScenario() => SceneFlow.StartScenario();
        public void Quit() => Application.Quit();
    }
}
