using Code.Common.Logger.Service;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.AssetManagement.Behaviours;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private ContentLoaderBehaviour _loaderBehaviour;

        private IGameStateMachine _gameStateMachine;
        private ILoggerService _loggerService;
        private IAssetDownloadService _downloadService;

        [Inject]
        private void Construct(IGameStateMachine gameStateMachine, ILoggerService loggerService, IAssetDownloadService assetDownloadService)
        {
            _downloadService = assetDownloadService;
            _loggerService = loggerService;
            _gameStateMachine = gameStateMachine;
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
            Initialize().Forget();
        }

        private async UniTaskVoid Initialize()
        {
            await _downloadService.InitializeDownloadDataAsync();
            float downloadSize = _downloadService.GetDownloadSizeMb();

            Debug.Log($"DOWNLOAD SIZE IS {downloadSize} Mb");

            if (downloadSize > 0)
            {
                _loaderBehaviour.Init();
                await _downloadService.UpdateContentAsync();
                _loaderBehaviour.Hide();
            }

            _gameStateMachine.Enter<BootstrapState>();
            DontDestroyOnLoad(gameObject);
        }
    }
}