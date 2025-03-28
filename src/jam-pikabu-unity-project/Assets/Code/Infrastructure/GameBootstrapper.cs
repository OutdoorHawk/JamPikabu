using Code.Common.Logger.Service;
using Code.Infrastructure.AssetManagement.AssetDownload;
using Code.Infrastructure.AssetManagement.Behaviours;
using Code.Infrastructure.Integrations.Service;
using Code.Infrastructure.Intro;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.GameStates.Bootstrap;
using Code.Infrastructure.States.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private ContentLoaderBehaviour _loaderBehaviour;
        [SerializeField] private IntroAnimator _introAnimator;

        private IGameStateMachine _gameStateMachine;
        private ILoggerService _loggerService;
        private IAssetDownloadService _downloadService;
        private IIntegrationsService _integrationsService;

        [Inject]
        private void Construct
        (
            IGameStateMachine gameStateMachine,
            ILoggerService loggerService,
            IAssetDownloadService assetDownloadService,
            IIntegrationsService integrationsService
        )
        {
            _integrationsService = integrationsService;
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
            UniTask loadIntegrationsTask = _integrationsService.LoadIntegrations();
            await _downloadService.InitializeDownloadDataAsync();
            float downloadSize = _downloadService.GetDownloadSizeMb();

            _loggerService.Log($"DOWNLOAD SIZE IS {downloadSize} Mb");

            if (downloadSize > 0)
            {
                _loaderBehaviour.Init();
                await _downloadService.UpdateContentAsync();
            }

            await loadIntegrationsTask;
            
            _loaderBehaviour.Hide();
            _gameStateMachine.Enter<BootstrapState, BootstrapStatePayload>(new BootstrapStatePayload(_introAnimator));
            DontDestroyOnLoad(gameObject);
        }
    }
}