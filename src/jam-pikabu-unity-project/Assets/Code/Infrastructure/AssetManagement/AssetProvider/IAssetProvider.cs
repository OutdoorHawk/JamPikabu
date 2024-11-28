using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.AssetManagement.AssetProvider
{
    public interface IAssetProvider
    {
        UniTask Initialize();
        T LoadAssetFromResources<T>(string path) where T : Component;
        void Cleanup();
    }
}