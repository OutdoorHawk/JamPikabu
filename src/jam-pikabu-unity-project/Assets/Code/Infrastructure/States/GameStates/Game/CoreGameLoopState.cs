using Code.Gameplay.Features;
using Code.Gameplay.Input;
using Code.Gameplay.Windows.Factory;
using Code.Infrastructure.SceneContext;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.Systems;
using Zenject;

namespace Code.Infrastructure.States.GameStates.Game
{
    public class CoreGameLoopState : EndOfFrameExitState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly ISceneContextProvider _sceneContextProvider;
        private readonly ISystemFactory _systemFactory;
        private readonly IGameStateHandlerService _gameStateHandlerService;

        private CoreGameLoopFeature _gameLoopFeature;
        private CoreGameLoopPhysicsFeature _gameLoopPhysicsFeature;
        private InputFeature _inputFeature;

        [Inject]
        public CoreGameLoopState
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
            _systemFactory = systemFactory;
            _sceneContextProvider = sceneContextProvider;
        }

        public override void Enter()
        {
            base.Enter();

            _gameLoopFeature = _systemFactory.Create<CoreGameLoopFeature>();
            _gameLoopPhysicsFeature = _systemFactory.Create<CoreGameLoopPhysicsFeature>();
            _inputFeature = _systemFactory.Create<InputFeature>();

            _inputFeature.Initialize();
            _gameLoopFeature.Initialize();
            _gameLoopPhysicsFeature.Initialize();
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

            ClearReactive();
            CleanUp();
            TearDown();
            Clear();
        }

        private void ClearReactive()
        {
            _gameLoopFeature.DeactivateReactiveSystems();
            _gameLoopPhysicsFeature.DeactivateReactiveSystems();
            _inputFeature.DeactivateReactiveSystems();

            _inputFeature.ClearReactiveSystems();
            _gameLoopFeature.ClearReactiveSystems();
            _gameLoopPhysicsFeature.ClearReactiveSystems();
        }

        private void CleanUp()
        {
            _inputFeature.Cleanup();
            _gameLoopFeature.Cleanup();
            _gameLoopPhysicsFeature.Cleanup();
        }

        private void TearDown()
        {
            _gameLoopFeature.TearDown();
            _gameLoopPhysicsFeature.TearDown();
            _inputFeature.TearDown();
        }

        private void Clear()
        {
            _gameLoopFeature = null;
            _inputFeature = null;
            _gameLoopPhysicsFeature = null;
        }
    }
}