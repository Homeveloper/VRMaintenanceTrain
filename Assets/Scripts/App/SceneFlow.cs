using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace VRMaintenanceTrainer
{
    public static class SceneFlow
    {
        private const string WorkshopAddress = "Scenes/Workshop";
        private static AsyncOperationHandle<SceneInstance> _workshop;
        private static bool _busy;

        public static async void StartScenario()
        {
            if (_busy) return;
            _busy = true;
            try
            {
                _workshop = Addressables.LoadSceneAsync(WorkshopAddress, LoadSceneMode.Single);
                await _workshop.Task;
                if (_workshop.Status != AsyncOperationStatus.Succeeded)
                    Debug.LogError("Failed to load Workshop through Addressables.");
            }
            catch (Exception exception) { Debug.LogException(exception); }
            finally { _busy = false; }
        }

        public static async void ReturnToMenu()
        {
            if (_busy) return;
            _busy = true;
            try
            {
                if (_workshop.IsValid())
                {
                    var unload = Addressables.UnloadSceneAsync(_workshop, true);
                    await unload.Task;
                }
                await SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
            }
            catch (Exception exception) { Debug.LogException(exception); }
            finally { _busy = false; }
        }

        public static async void RestartScenario()
        {
            if (_busy) return;
            _busy = true;
            try
            {
                if (_workshop.IsValid())
                {
                    var unload = Addressables.UnloadSceneAsync(_workshop, true);
                    await unload.Task;
                }
                _workshop = Addressables.LoadSceneAsync(WorkshopAddress, LoadSceneMode.Single);
                await _workshop.Task;
                if (_workshop.Status != AsyncOperationStatus.Succeeded)
                    Debug.LogError("Failed to restart Workshop.");
            }
            catch (Exception exception) { Debug.LogException(exception); }
            finally { _busy = false; }
        }
    }
}
