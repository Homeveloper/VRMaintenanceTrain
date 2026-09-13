using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VRMaintenanceTrainer
{
    public sealed class ScenarioHud : MonoBehaviour
    {
        [SerializeField] private ScenarioController scenario;
        [SerializeField] private MaintenanceTool tool;
        [SerializeField] private TMP_Text instruction;
        [SerializeField] private TMP_Text message;
        [SerializeField] private TMP_Text result;
        [SerializeField] private Image progressFill;
        [SerializeField] private GameObject resultPanel;

        private float _messageUntil;

        private void OnEnable()
        {
            scenario.StepChanged += ShowStep;
            scenario.Mistake += ShowMistake;
            scenario.Completed += ShowResult;
        }

        private void OnDisable()
        {
            scenario.StepChanged -= ShowStep;
            scenario.Mistake -= ShowMistake;
            scenario.Completed -= ShowResult;
        }

        private void Start()
        {
            ShowStep(scenario.CurrentHint);
            if (resultPanel != null) resultPanel.SetActive(false);
        }

        private void Update()
        {
            if (message != null && Time.time > _messageUntil) message.text = "";
            if (progressFill != null) progressFill.fillAmount = scenario.StepIndex == 2 ? tool.Progress01 : 0f;
        }

        private void ShowStep(string hint)
        {
            if (instruction != null) instruction.text = hint;
            if (message != null) message.text = "";
        }

        private void ShowMistake(string hint)
        {
            if (message != null) message.text = hint;
            _messageUntil = Time.time + 3.5f;
        }

        private void ShowResult()
        {
            if (instruction != null) instruction.text = "Maintenance complete";
            if (result != null) result.text = "Task complete";
            if (resultPanel != null) resultPanel.SetActive(true);
        }

        public void Restart() => SceneFlow.RestartScenario();
        public void Menu() => SceneFlow.ReturnToMenu();
    }
}
