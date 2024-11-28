using Code.Common.Logger.Service;
using Cysharp.Threading.Tasks;
using UnityEngine;

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
    }
}