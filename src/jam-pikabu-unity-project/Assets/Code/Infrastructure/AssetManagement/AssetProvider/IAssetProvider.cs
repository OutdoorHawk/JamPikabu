using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.AssetManagement.AssetProvider
{
    public interface IAssetProvider
    {
        UniTask Initialize();
        UniTask<GameObject> LoadGameObjectAsync(string path);
        UniTask<T> LoadAssetAsync<T>(string path) where T : Object;
        UniTask<IList<T>> LoadAssetsAsync<T>(string label) where T : Object;
        T LoadAssetFromResources<T>(string path) where T : Component;
        void Cleanup();
    }
}