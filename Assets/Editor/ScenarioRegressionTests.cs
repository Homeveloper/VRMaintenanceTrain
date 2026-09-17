using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace VRMaintenanceTrainer.Editor
{
    public sealed class ScenarioRegressionTests
    {
        [MenuItem("Tools/Run Scenario Regression Tests")]
        public static void RunAll()
        {
            Run(nameof(ToolTimeBeforePartInstallationDoesNotCount), test => test.ToolTimeBeforePartInstallationDoesNotCount());
            Run(nameof(ReleasingActivationResetsToolTime), test => test.ReleasingActivationResetsToolTime());
            Run(nameof(EarlyToolUseShowsOneHintPerStep), test => test.EarlyToolUseShowsOneHintPerStep());
            Debug.Log("Scenario regression tests passed: 3/3");
        }

        private static void Run(string name, System.Action<ScenarioRegressionTests> action)
        {
            var test = new ScenarioRegressionTests();
            try
            {
                test.SetUp();
                action(test);
                Debug.Log($"Passed: {name}");
            }
            finally
            {
                test.TearDown();
            }
        }

        private GameObject _scenarioObject;
        private GameObject _toolObject;
        private ScenarioController _scenario;
        private MaintenanceTool _tool;
        private MethodInfo _advance;

        [SetUp]
        public void SetUp()
        {
            _scenarioObject = new GameObject("TestScenario");
            _scenario = _scenarioObject.AddComponent<ScenarioController>();

            _toolObject = new GameObject("TestTool");
            _toolObject.SetActive(false);
            _toolObject.AddComponent<XRGrabInteractable>();
            _tool = _toolObject.AddComponent<MaintenanceTool>();

            var scenarioField = typeof(MaintenanceTool).GetField("scenario", BindingFlags.Instance | BindingFlags.NonPublic);
            _advance = typeof(MaintenanceTool).GetMethod("Advance", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(scenarioField);
            Assert.IsNotNull(_advance);
            scenarioField.SetValue(_tool, _scenario);
            _toolObject.SetActive(true);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_toolObject);
            Object.DestroyImmediate(_scenarioObject);
        }

        [Test]
        public void ToolTimeBeforePartInstallationDoesNotCount()
        {
            _tool.SetDesktopState(true, true, true);
            Advance(1.9f);
            Assert.AreEqual(0f, _tool.Progress01);
            Assert.AreEqual(0, _scenario.StepIndex);

            Assert.IsTrue(_scenario.TryPerform(new ScenarioAction(ScenarioActionType.PowerOff)));
            Advance(1.9f);
            Assert.AreEqual(0f, _tool.Progress01);
            Assert.AreEqual(1, _scenario.StepIndex);

            Assert.IsTrue(_scenario.TryPerform(new ScenarioAction(ScenarioActionType.PartInstalled, "replacement")));
            Advance(0.1f);
            Assert.IsFalse(_scenario.IsComplete);
            Assert.That(_tool.Progress01, Is.GreaterThan(0f).And.LessThan(0.1f));

            Advance(1.8f);
            Assert.IsFalse(_scenario.IsComplete);
            Advance(0.2f);
            Assert.IsTrue(_scenario.IsComplete);
        }

        [Test]
        public void ReleasingActivationResetsToolTime()
        {
            _scenario.TryPerform(new ScenarioAction(ScenarioActionType.PowerOff));
            _scenario.TryPerform(new ScenarioAction(ScenarioActionType.PartInstalled, "replacement"));

            _tool.SetDesktopState(true, true, true);
            Advance(1.5f);
            Assert.That(_tool.Progress01, Is.GreaterThan(0.7f));

            _tool.SetDesktopState(true, true, false);
            Advance(0.01f);
            Assert.AreEqual(0f, _tool.Progress01);

            _tool.SetDesktopState(true, true, true);
            Advance(1.9f);
            Assert.IsFalse(_scenario.IsComplete);
            Advance(0.2f);
            Assert.IsTrue(_scenario.IsComplete);
        }

        [Test]
        public void EarlyToolUseShowsOneHintPerStep()
        {
            var mistakes = 0;
            _scenario.Mistake += _ => mistakes++;
            _tool.SetDesktopState(true, true, true);

            Advance(0.5f);
            Advance(0.5f);
            Assert.AreEqual(1, mistakes);

            _scenario.TryPerform(new ScenarioAction(ScenarioActionType.PowerOff));
            Advance(0.5f);
            Advance(0.5f);
            Assert.AreEqual(2, mistakes);
            Assert.AreEqual(0f, _tool.Progress01);
        }

        private void Advance(float seconds) => _advance.Invoke(_tool, new object[] { seconds });
    }
}
