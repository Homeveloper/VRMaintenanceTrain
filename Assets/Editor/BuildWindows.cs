using System;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace VRMaintenanceTrainer.Editor
{
    public static class BuildWindows
    {
        [MenuItem("Tools/Build Windows Trainer")]
        public static void Build()
        {
            AddressableAssetSettings.BuildPlayerContent(out var addressablesResult);
            if (addressablesResult == null || !string.IsNullOrEmpty(addressablesResult.Error))
                throw new InvalidOperationException(
                    $"Addressables build failed: {addressablesResult?.Error ?? "No build result"}");

            var output = Path.GetFullPath("Builds/Windows/VRMaintenanceTrainer.exe");
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            var options = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/MainMenu.unity" },
                locationPathName = output,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException($"Windows build failed: {report.summary.result}");

            Debug.Log($"Windows build completed: {output}");
        }
    }
}
