using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Common.Logger.Service;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace Code.Infrastructure.AssetManagement.AssetProvider
{
    public class AssetProvider : IAssetProvider
    {
        private readonly ILoggerService _logger;

        private const string GAME_STATIC_DATA_PATH = "GameStaticData";
        private const string LOG_COLOR = "orange";

        public AssetProvider(ILoggerService logger)
        {
            _logger = logger;
        }

        public UniTask Initialize()
        {
            return UniTask.CompletedTask;
        }
        
        public async UniTask<GameObject> LoadGameObjectAsync(string path)
        {
            _logger.Log($"<b><color={LOG_COLOR}>[AssetProvider]</color></b> Loading resource: {path}");
            var handle = Addressables.LoadAssetAsync<GameObject>(path);
            handle.Completed += _ => LogResult(path, handle);
            return await handle.ToUniTask();
        }

        public async UniTask<T> LoadAssetAsync<T>(string path) where T : Object
        {
            _logger.Log($"<b><color={LOG_COLOR}>[AssetProvider]</color></b> Loading resource: {path}");
            var handle = Addressables.LoadAssetAsync<T>(path);
            handle.Completed += _ => LogResult(path, handle);
            return await handle.ToUniTask();
        }

        public async UniTask<IList<T>> LoadAssetsAsync<T>(string label) where T : Object
        {
            _logger.Log($"<b><color={LOG_COLOR}>[AssetProvider]</color></b> Loading resources by label: {label}");
            var handle = Addressables.LoadAssetsAsync<T>(label, null);
            handle.Completed += _ => LogResult(label, handle);
            return await handle.ToUniTask();
        }

        public T LoadAssetFromResources<T>(string path) where T : Component
        {
            _logger.Log($"<b><color={LOG_COLOR}>[AssetProvider]</color></b> Loading resource: {path}");
            T resource = Resources.Load<T>(path);
            _logger.Log($"<b><color={LOG_COLOR}>[AssetProvider]</color></b> Loading complete: {path}");
            return resource;
        }

        public void Cleanup()
        {
        }

        private void LogResult<T>(string path, AsyncOperationHandle<T> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
                _logger.Log($"<b><color={LOG_COLOR}>[AssetProvider]</color></b> Loading complete: {path}");
            else
                _logger.LogError($"<b><color={LOG_COLOR}>[AssetProvider]</color></b> Error loading {path}");
        }
    }
}