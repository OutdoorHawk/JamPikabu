using System.Collections.Generic;
using Code.Common.Logger.Service;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Code.Infrastructure.AssetManagement.AssetProvider
{
    public class AssetProvider : IAssetProvider
    {
        private readonly ILoggerService _logger;

        private const string GAME_STATIC_DATA_PATH = "GameStaticData";

        private readonly Dictionary<string, AsyncOperationHandle> _staticCache = new();
        private readonly Dictionary<string, List<AsyncOperationHandle>> _handles = new();
        private readonly Dictionary<string, AsyncOperationHandle> _levelContextCache = new();

        private readonly List<AsyncOperationHandle> _activeLoadings = new();

        private const string LOG_COLOR = "orange";

        public AssetProvider(ILoggerService logger)
        {
            _logger = logger;
        }

        public IReadOnlyList<AsyncOperationHandle> ActiveLoadings => _activeLoadings;

        public void LoadGameStaticData()
        {
        }

        public async UniTask Initialize()
        {
            var handle = Addressables.InitializeAsync();
            await WaitForInitTask(handle);
        }

        public async UniTask<T> Load<T>(AssetReference assetReference, bool isStaticData = false) where T : class
        {
            return isStaticData
                ? await LoadFromCache<T>(assetReference, _staticCache)
                : await LoadFromCache<T>(assetReference, _levelContextCache);
        }

        public async UniTask<T> Load<T>(string address, bool isStaticData = false) where T : class
        {
            return isStaticData
                ? await LoadFromCache<T>(address, _staticCache)
                : await LoadFromCache<T>(address, _levelContextCache);
        }

        public T LoadAssetFromResouses<T>(string path) where T : Component
        {
            return Resources.Load<T>(path);
        }

        private async UniTask WaitForInitTask(AsyncOperationHandle<IResourceLocator> handle)
        {
            UniTaskCompletionSource waitMoveSource = new();
            UniTask waitMoveTask = waitMoveSource.Task;

            handle.Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    _logger.Log($"<b><color={LOG_COLOR}>[Addressable]</color></b> Initialized successfully.");
                }
                else
                {
                    _logger.LogError($"<b><color={LOG_COLOR}>[Addressable]</color></b> Initialization failed.");
                }

                waitMoveSource.TrySetResult();
            };

            await waitMoveTask;
        }

        private async UniTask<T> LoadFromCache<T>(AssetReference assetReference, Dictionary<string, AsyncOperationHandle> cache) where T : class
        {
            if (cache.TryGetValue(assetReference.AssetGUID, out AsyncOperationHandle completedHandle))
                return completedHandle.Result as T;

            return await RunWithCacheOnComplete(Addressables.LoadAssetAsync<T>(assetReference),
                assetReference.AssetGUID, cache);
        }

        private async UniTask<T> LoadFromCache<T>(string address, Dictionary<string, AsyncOperationHandle> cache)
            where T : class
        {
            if (cache.TryGetValue(address, out AsyncOperationHandle completedHandle))
                return completedHandle.Result as T;

            _logger.Log($"<b><color={LOG_COLOR}>[Addressable]</color></b> Loading asset: {address}");
            return await RunWithCacheOnComplete(Addressables.LoadAssetAsync<T>(address), address, cache);
        }

        private async UniTask<T> RunWithCacheOnComplete<T>(AsyncOperationHandle<T> handle, string cacheKey,
            IDictionary<string, AsyncOperationHandle> cache) where T : class
        {
            _activeLoadings.Add(handle);

            handle.Completed += completeHandle =>
            {
                cache[cacheKey] = completeHandle;
                _logger.Log($"<b><color={LOG_COLOR}>[Addressable]</color></b> Load asset complete: {cacheKey}");
                _activeLoadings.Remove(handle);
            };

            AddHandle(cacheKey, handle);
            return await handle.Task;
        }

        private void AddHandle(string key, AsyncOperationHandle handle)
        {
            if (_handles.TryGetValue(key, out List<AsyncOperationHandle> resourceHandles) == false)
            {
                resourceHandles = new List<AsyncOperationHandle>();
                _handles[key] = resourceHandles;
            }

            resourceHandles.Add(handle);
        }

        public void LevelContextAssetsCleanup()
        {
            foreach (KeyValuePair<string, AsyncOperationHandle> handlePair in _levelContextCache)
            {
                _handles.Remove(handlePair.Key);
                Addressables.Release(handlePair.Value);
            }

            _levelContextCache.Clear();
        }

        public void Cleanup()
        {
            foreach (List<AsyncOperationHandle> resourceHandles in _handles.Values)
            foreach (AsyncOperationHandle handle in resourceHandles)
                Addressables.Release(handle);

            _staticCache.Clear();
            _levelContextCache.Clear();
            _handles.Clear();
        }
    }
}