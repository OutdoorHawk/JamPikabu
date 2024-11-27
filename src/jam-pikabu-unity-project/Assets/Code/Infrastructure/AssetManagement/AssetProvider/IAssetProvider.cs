using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Gameplay.StaticData;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Code.Infrastructure.AssetManagement.AssetProvider
{
    public interface IAssetProvider
    {
        IReadOnlyList<AsyncOperationHandle> ActiveLoadings { get; }
        UniTask Initialize();
        UniTask<T> Load<T>(AssetReference assetReference, bool isStaticData = false) where T : class;
        UniTask<T> Load<T>(string address, bool isStaticData = false) where T : class;
        T LoadAssetFromResouses<T>(string path) where T : Component;
        void LevelContextAssetsCleanup();
        void Cleanup();
    }
}