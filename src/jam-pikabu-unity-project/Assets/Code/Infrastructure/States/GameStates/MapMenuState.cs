using Code.Gameplay.Windows.Service;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.Systems;
using Code.Meta.Features;
using Code.Progress.SaveLoadService;

namespace Code.Infrastructure.States.GameStates
{
    public class MapMenuState : EndOfFrameExitState
    {
        private readonly IWindowService _windowService;
        private readonly ISystemFactory _systemFactory;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IGameStateHandlerService _gameStateHandlerService;

        private MapMenuFeature _mapMenuFeature;

        public MapMenuState
        (
            IGameStateMachine gameStateMachine,
            IWindowService windowService,
            ISystemFactory systemFactory,
            ISaveLoadService saveLoadService,
            IGameStateHandlerService gameStateHandlerService
        )
        {
            _gameStateHandlerService = gameStateHandlerService;
            _saveLoadService = saveLoadService;
            _gameStateMachine = gameStateMachine;
            _windowService = windowService;
            _systemFactory = systemFactory;
        }

        public override void Enter()
        {
            _mapMenuFeature = _systemFactory.Create<MapMenuFeature>();

            _mapMenuFeature.Initialize();
            _saveLoadService.SaveProgress();

            _gameStateHandlerService.OnEnterMainMenu();
        }

        protected override void OnUpdate()
        {
            _mapMenuFeature.Execute();
            _mapMenuFeature.Cleanup();
        }

        protected override void ExitOnEndOfFrame()
        {
            _windowService.ClearUIRoot();

            _mapMenuFeature.DeactivateReactiveSystems();
            _mapMenuFeature.ClearReactiveSystems();

            _mapMenuFeature.Cleanup();
            _mapMenuFeature.TearDown();
        }
    }
}