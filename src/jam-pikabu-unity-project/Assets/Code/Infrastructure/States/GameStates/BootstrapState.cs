using System.Collections.Generic;
using Code.Common.Logger.Service;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows.Factory;
using Code.Infrastructure.AssetManagement.AssetProvider;
using Code.Infrastructure.Integrations;
using Code.Infrastructure.Localization;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Zenject;

namespace Code.Infrastructure.States.GameStates
{
    public class BootstrapState : SimpleState
    {
        private readonly IUIFactory _uiFactory;
        private readonly IStaticDataService _staticDataService;
        private readonly IAssetProvider _assetProvider;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IGameStateHandlerService _gameStateHandlerService;
        private readonly ILoggerService _loggerService;
        private readonly ILocalizationService _localizationService;

        [Inject]
        public BootstrapState
        (
            IGameStateMachine gameStateMachine,
            IUIFactory uiFactory,
            IStaticDataService staticDataService,
            IAssetProvider assetProvider,
            ILocalizationService localizationService,
            IGameStateHandlerService gameStateHandlerService,
            ILoggerService loggerService,
            List<IIntegration> integrations
        )
        {
            _localizationService = localizationService;
            _loggerService = loggerService;
            _gameStateHandlerService = gameStateHandlerService;
            _gameStateMachine = gameStateMachine;
            _assetProvider = assetProvider;
            _staticDataService = staticDataService;
            _uiFactory = uiFactory;
        }

        public override async void Enter()
        {
            base.Enter();
            await LoadServiceData();
            _gameStateHandlerService.OnEnterBootstrapState();
            OnLoaded();
        }

        private async UniTask LoadServiceData()
        {
            _loggerService.Log("<b>[Bootstrap]</b> Init assetProvider");
            await _assetProvider.Initialize();
            _loggerService.Log("<b>[Bootstrap]</b> Init staticData");
            await _staticDataService.Load();
            _loggerService.Log("<b>[Bootstrap]</b> CreateUiRoot");
            _uiFactory.CreateUiRoot();
            _loggerService.Log("<b>[Bootstrap]</b> Init localization");
            await _localizationService.Initialize();
            _loggerService.Log("<b>[Bootstrap]</b> SetTweensCapacity");
            DOTween.SetTweensCapacity(300, 250);
            _loggerService.Log("<b>[Bootstrap]</b> Bootstrap complete");
        }

        private void OnLoaded()
        {
            _gameStateMachine.Enter<LoadProgressState>();
        }
    }
}