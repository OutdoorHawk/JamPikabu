using System;
using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Common.Logger.Service;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Code.Infrastructure.AssetManagement.AssetDownload
{
    public class LabeledAssetDownloadService : IAssetDownloadService
    {
        private const string RemoteLabel = "ProdStaticData";
        private const string RemoteCatalogPath = "https://s3.eponesh.com/games/files/18994/catalog_0.1.1.json";
        private const string RemoteHashPath = "https://s3.eponesh.com/games/files/18994/catalog_0.1.1.hash";
        private const string LocalCatalogPath = "https://html-classic.itch.zone/html/12240601/Pikabu/StreamingAssets/aa/catalog.json";
        
        private const bool EnableLocalCatalogOverride = true;

        private readonly IAssetDownloadReporter _downloadReporter;
        private readonly ILoggerService _loggerService;
        private long _downloadSize;
        private bool _remoteCatalogAvailable = true;

        public LabeledAssetDownloadService(IAssetDownloadReporter downloadReporter, ILoggerService loggerService)
        {
            _downloadReporter = downloadReporter;
            _loggerService = loggerService;
        }

        public async UniTask InitializeDownloadDataAsync()
        {
            try
            {
                Addressables.WebRequestOverride = EditWebRequestUrl;
                await CheckRemoteCatalogAvailability();
                Addressables.InternalIdTransformFunc = EditWebUrl;
                await Addressables.InitializeAsync().ToUniTask();
                await UpdateCatalogsAsync();
                await UpdateDownloadSizeAsync();
            }
            catch (Exception e)
            {
                _loggerService.LogError($"Error updating catalogs {e}");
            }
        }

        public float GetDownloadSizeMb() =>
            SizeToMb(_downloadSize);

        public async UniTask UpdateContentAsync()
        {
            try
            {
                AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(RemoteLabel);

                while (!downloadHandle.IsDone && downloadHandle.IsValid())
                {
                    await UniTask.Delay(100);
                    _downloadReporter.Report(downloadHandle.GetDownloadStatus().Percent);
                }

                _downloadReporter.Report(1);

                if (downloadHandle.Status == AsyncOperationStatus.Failed)
                    Debug.LogError("Error while downloading catalog dependencies");

                if (downloadHandle.IsValid())
                    Addressables.Release(downloadHandle);

                _downloadReporter.Reset();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        private async UniTask CheckRemoteCatalogAvailability()
        {
            try
            {
                UnityWebRequest request = UnityWebRequest.Head(RemoteCatalogPath);
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    _remoteCatalogAvailable = true;
                    _loggerService.Log($"Remote catalog is available: {RemoteCatalogPath}");
                }
                else
                {
                    _remoteCatalogAvailable = false;
                    _loggerService.Log($"Remote catalog is unavailable. Falling back to local catalog.");
                }
            }
            catch (Exception e)
            {
                _remoteCatalogAvailable = false;
                _loggerService.Log($"Error checking remote catalog availability: {e.Message}");
            }
        }

        private void EditWebRequestUrl(UnityWebRequest request)
        {
            if
            (
                request.url.StartsWith("https") &&
                (request.url.EndsWith(".json") ||
                 request.url.EndsWith(".hash"))
            )
            {
                request.url = request.url + "?t=" + DateTime.UtcNow.Ticks;
            }

            _loggerService.Log($"WebRequestOverride URL: {request.url}");
        }

        private string EditWebUrl(IResourceLocation location)
        {
            _loggerService.Log($"Requesting: {location.InternalId} remoteCatalogAvailable: {_remoteCatalogAvailable}");

            if (EnableLocalCatalogOverride == false)
                return location.InternalId;
            
            if (_remoteCatalogAvailable == false)
                return location.InternalId;
            
            if (location.InternalId.Contains("catalog.json"))
            {
                _loggerService.Log($"Requesting: {location.InternalId} | Edit to {RemoteCatalogPath}");
                return RemoteCatalogPath;
            }

            if (location.InternalId.Contains("catalog.hash"))
            {
                _loggerService.Log($"Requesting: {location.InternalId} | Edit to {RemoteHashPath}");
                return RemoteHashPath;
            }
            
            return location.InternalId;
        }

        private async UniTask UpdateCatalogsAsync()
        {
            await ClearDependencyCache(RemoteLabel);

            _loggerService.Log($"Local catalog path: {LocalCatalogPath}");
            _loggerService.Log($"Application.persistentDataPath: {Application.persistentDataPath}");
            _loggerService.Log($"Remote catalog path: {RemoteCatalogPath}");

            List<string> catalogsToUpdate = await Addressables.CheckForCatalogUpdates().ToUniTask();

            _loggerService.Log($"Catalogs to update: {string.Join(", ", catalogsToUpdate)}");

            if (catalogsToUpdate.IsNullOrEmpty())
                return;

            await Addressables.UpdateCatalogs(catalogsToUpdate).ToUniTask();

            _loggerService.Log($"Catalogs to update: {string.Join(", ", catalogsToUpdate)}");
        }

        private static async UniTask ClearDependencyCache(string path)
        {
            UniTaskCompletionSource source = new UniTaskCompletionSource();

            Addressables.ClearDependencyCacheAsync(path, true).Completed += _ =>
            {
                source.TrySetResult();
                Debug.Log($"Cleared cached dependencies for the path: {path}");
            };

            await source.Task;
        }

        private async UniTask UpdateDownloadSizeAsync()
        {
            _downloadSize = await Addressables
                .GetDownloadSizeAsync(RemoteLabel)
                .ToUniTask();
        }

        private static float SizeToMb(long downloadSize) => downloadSize * 1f / 1048576;
    }
}