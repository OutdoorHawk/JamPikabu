using System.Collections.Generic;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.Systems;
using Code.Meta.Features;
using Code.Meta.UI.HardCurrencyHolder.Service;
using Code.Progress.SaveLoadService;

namespace Code.Infrastructure.States.GameStates
{
    public class MainMenuState : EndOfFrameExitState
    {
        private readonly IWindowService _windowService;
        private readonly IStorageUIService _storageUIService;
        private readonly ISystemFactory _systemFactory;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IGameStateHandlerService _gameStateHandlerService;

        private MainMenuFeature _mainMenuFeature;

        public MainMenuState
        (
            IGameStateMachine gameStateMachine,
            IWindowService windowService,
            IStorageUIService storageUIService,
            ISystemFactory systemFactory,
            ISaveLoadService saveLoadService,
            IGameStateHandlerService gameStateHandlerService
        )
        {
            _gameStateHandlerService = gameStateHandlerService;
            _saveLoadService = saveLoadService;
            _gameStateMachine = gameStateMachine;
            _windowService = windowService;
            _storageUIService = storageUIService;
            _systemFactory = systemFactory;
        }

        public override void Enter()
        {
            _mainMenuFeature = _systemFactory.Create<MainMenuFeature>();

            _mainMenuFeature.Initialize();
            _saveLoadService.SaveProgress();
            
            _gameStateHandlerService.OnEnterMainMenu();
        }

        protected override void OnUpdate()
        {
            _mainMenuFeature.Execute();
            _mainMenuFeature.Cleanup();
        }

        protected override void ExitOnEndOfFrame()
        {
            _storageUIService.Cleanup();
            _windowService.ClearUIRoot();

            _mainMenuFeature.DeactivateReactiveSystems();
            _mainMenuFeature.ClearReactiveSystems();

            _mainMenuFeature.Cleanup();
            _mainMenuFeature.TearDown();
        }
    }
}