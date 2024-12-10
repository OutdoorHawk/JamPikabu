using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.AssetManagement.AssetProvider
{
    public interface IAssetProvider
    {
        UniTask Initialize();
        UniTask<GameObject> LoadAsset(string path);
        T LoadAssetFromResources<T>(string path) where T : Component;
        void Cleanup();
    }
}