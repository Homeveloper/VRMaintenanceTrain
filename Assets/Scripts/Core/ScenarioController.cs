using System;
using UnityEngine;

namespace VRMaintenanceTrainer
{
    public enum ScenarioActionType { PowerOff, PartInstalled, ToolApplied }

    public readonly struct ScenarioAction
    {
        public readonly ScenarioActionType Type;
        public readonly string SocketId;

        public ScenarioAction(ScenarioActionType type, string socketId = null)
        {
            Type = type;
            SocketId = socketId;
        }
    }

    public interface IScenarioStep
    {
        string Hint { get; }
        bool Accepts(in ScenarioAction action);
    }

    internal sealed class PowerOffStep : IScenarioStep
    {
        public string Hint => "Отключите питание рубильником";
        public bool Accepts(in ScenarioAction action) => action.Type == ScenarioActionType.PowerOff;
    }

    internal sealed class InstallPartStep : IScenarioStep
    {
        public string Hint => "Установите сменную деталь в зелёное крепление";
        public bool Accepts(in ScenarioAction action) =>
            action.Type == ScenarioActionType.PartInstalled && action.SocketId == "replacement";
    }

    internal sealed class ApplyToolStep : IScenarioStep
    {
        public string Hint => "Удерживайте инструмент в рабочей зоне 2 секунды";
        public bool Accepts(in ScenarioAction action) => action.Type == ScenarioActionType.ToolApplied;
    }

    public sealed class ScenarioController : MonoBehaviour
    {
        private readonly IScenarioStep[] _steps = { new PowerOffStep(), new InstallPartStep(), new ApplyToolStep() };
        private int _stepIndex;

        public event Action<string> StepChanged;
        public event Action<string> Mistake;
        public event Action Completed;

        public int StepIndex => _stepIndex;
        public bool IsComplete => _stepIndex >= _steps.Length;
        public string CurrentHint => IsComplete ? "Обслуживание завершено" : _steps[_stepIndex].Hint;

        public bool CanPerform(in ScenarioAction action) =>
            !IsComplete && _steps[_stepIndex].Accepts(action);

        private void Start() => StepChanged?.Invoke(CurrentHint);

        public bool TryPerform(in ScenarioAction action)
        {
            if (IsComplete) return false;

            if (!CanPerform(action))
            {
                var detail = action.Type == ScenarioActionType.PartInstalled &&
                             _stepIndex == 1 && action.SocketId != "replacement"
                    ? "Крепление не подходит. "
                    : "Неверный порядок действий. ";
                Mistake?.Invoke(detail + CurrentHint + ".");
                return false;
            }

            _stepIndex++;
            if (IsComplete) Completed?.Invoke();
            else StepChanged?.Invoke(CurrentHint);
            return true;
        }
    }
}
