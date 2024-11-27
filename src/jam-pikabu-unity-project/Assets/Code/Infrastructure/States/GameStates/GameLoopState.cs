using Code.Gameplay.Features;
using Code.Gameplay.Initialize;
using Code.Gameplay.Input;
using Code.Gameplay.Windows.Factory;
using Code.Infrastructure.SceneContext;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.Systems;
using Zenject;

namespace Code.Infrastructure.States.GameStates
{
    public class GameLoopState : EndOfFrameExitState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly ISceneContextProvider _sceneContextProvider;
        private readonly IInitializationService _initializationService;
        private readonly ISystemFactory _systemFactory;
        private readonly GameContext _gameContext;
        private readonly InputContext _inputContext;
        private readonly IGameStateHandlerService _gameStateHandlerService;

        private BattleFeature _battleFeature;
        private BattlePhysicsFeature _battlePhysicsFeature;
        private InputFeature _inputFeature;

        [Inject]
        public GameLoopState
        (
            IGameStateMachine gameStateMachine,
            IUIFactory uiFactory,
            ISceneContextProvider sceneContextProvider,
            IInitializationService initializationService,
            ISystemFactory systemFactory,
            GameContext gameContext,
            InputContext inputContext,
            IGameStateHandlerService gameStateHandlerService
        )
        {
            _gameStateHandlerService = gameStateHandlerService;
            _inputContext = inputContext;
            _gameContext = gameContext;
            _systemFactory = systemFactory;
            _initializationService = initializationService;
            _sceneContextProvider = sceneContextProvider;
        }

        public override void Enter()
        {
            base.Enter();

            _battleFeature = _systemFactory.Create<BattleFeature>();
            _battlePhysicsFeature = _systemFactory.Create<BattlePhysicsFeature>();
            _inputFeature = _systemFactory.Create<InputFeature>();

            _inputFeature.Initialize();
            _battleFeature.Initialize();
            _battlePhysicsFeature.Initialize();

            _initializationService.SetLevelPrepared();

            _gameStateHandlerService.OnEnterGameLoop();
        }

        protected override void OnFixedUpdate()
        {
            _battlePhysicsFeature.Execute();
            _battlePhysicsFeature.Cleanup();
        }

        protected override void OnUpdate()
        {
            _inputFeature.Execute();
            _inputFeature.Cleanup();

            _battleFeature.Execute();
            _battleFeature.Cleanup();
        }

        protected override void ExitOnEndOfFrame()
        {
            base.ExitOnEndOfFrame();
            
            _sceneContextProvider.CleanUp();
            _initializationService.SetLevelNotPrepared();

            _battleFeature.DeactivateReactiveSystems();
            _battlePhysicsFeature.DeactivateReactiveSystems();
            _inputFeature.DeactivateReactiveSystems();
            _inputFeature.ClearReactiveSystems();
            _battleFeature.ClearReactiveSystems();
            _battlePhysicsFeature.ClearReactiveSystems();

            _inputFeature.Cleanup();
            _battleFeature.Cleanup();
            _battlePhysicsFeature.Cleanup();

            _battleFeature = null;
            _inputFeature = null;
            _battlePhysicsFeature = null;

            _gameStateHandlerService.OnExitGameLoop();
        }
    }
}