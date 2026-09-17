using System;
using System.Threading.Tasks;
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
                await LoadWorkshopAsync("Failed to load Workshop through Addressables.");
            }
            finally { _busy = false; }
        }

        public static async void ReturnToMenu()
        {
            if (_busy) return;
            _busy = true;
            try
            {
                await UnloadWorkshopAsync();
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
                await UnloadWorkshopAsync();
                await LoadWorkshopAsync("Failed to restart Workshop.");
            }
            catch (Exception exception) { Debug.LogException(exception); }
            finally { _busy = false; }
        }

        private static async Task<bool> LoadWorkshopAsync(string errorMessage)
        {
            AsyncOperationHandle<SceneInstance> load = default;
            var loaded = false;
            try
            {
                load = Addressables.LoadSceneAsync(WorkshopAddress, LoadSceneMode.Single);
                await load.Task;
                if (load.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError(errorMessage);
                    return false;
                }

                _workshop = load;
                loaded = true;
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return false;
            }
            finally
            {
                if (!loaded && load.IsValid())
                {
                    try { Addressables.Release(load); }
                    catch (Exception exception) { Debug.LogException(exception); }
                }
            }
        }

        private static async Task UnloadWorkshopAsync()
        {
            if (!_workshop.IsValid()) return;

            var unload = Addressables.UnloadSceneAsync(_workshop, true);
            await unload.Task;
            _workshop = default;
        }
    }
}
