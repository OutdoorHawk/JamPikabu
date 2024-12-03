using Code.Gameplay.Features;
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
        private readonly ISystemFactory _systemFactory;
        private readonly GameContext _gameContext;
        private readonly InputContext _inputContext;
        private readonly IGameStateHandlerService _gameStateHandlerService;

        private GameLoopFeature _gameLoopFeature;
        private GameLoopPhysicsFeature _gameLoopPhysicsFeature;
        private InputFeature _inputFeature;

        [Inject]
        public GameLoopState
        (
            IGameStateMachine gameStateMachine,
            IUIFactory uiFactory,
            ISceneContextProvider sceneContextProvider,
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
            _sceneContextProvider = sceneContextProvider;
        }

        public override void Enter()
        {
            base.Enter();

            _gameLoopFeature = _systemFactory.Create<GameLoopFeature>();
            _gameLoopPhysicsFeature = _systemFactory.Create<GameLoopPhysicsFeature>();
            _inputFeature = _systemFactory.Create<InputFeature>();

            _inputFeature.Initialize();
            _gameLoopFeature.Initialize();
            _gameLoopPhysicsFeature.Initialize();

            _gameStateHandlerService.OnEnterGameLoop();
        }

        protected override void OnFixedUpdate()
        {
            _gameLoopPhysicsFeature.Execute();
            _gameLoopPhysicsFeature.Cleanup();
        }

        protected override void OnUpdate()
        {
            _inputFeature.Execute();
            _inputFeature.Cleanup();

            _gameLoopFeature.Execute();
            _gameLoopFeature.Cleanup();
        }

        protected override void ExitOnEndOfFrame()
        {
            base.ExitOnEndOfFrame();
            
            _sceneContextProvider.CleanUp();

            _gameLoopFeature.DeactivateReactiveSystems();
            _gameLoopPhysicsFeature.DeactivateReactiveSystems();
            _inputFeature.DeactivateReactiveSystems();
            _inputFeature.ClearReactiveSystems();
            _gameLoopFeature.ClearReactiveSystems();
            _gameLoopPhysicsFeature.ClearReactiveSystems();

            _inputFeature.Cleanup();
            _gameLoopFeature.Cleanup();
            _gameLoopPhysicsFeature.Cleanup();

            _gameLoopFeature = null;
            _inputFeature = null;
            _gameLoopPhysicsFeature = null;

            _gameStateHandlerService.OnExitGameLoop();
        }
    }
}