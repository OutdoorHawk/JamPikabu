using Code.Gameplay.Features;
using Code.Gameplay.Input;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.Systems;

namespace Code.Infrastructure.States.GameStates
{
    public class GameOverState : EndOfFrameExitState
    {
        private readonly IWindowService _windowService;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISystemFactory _systemFactory;
        private readonly GameContext _gameContext;
        private readonly InputContext _inputContext;
        
        private GameLoopFeature _gameLoopFeature;
        private GameLoopPhysicsFeature _gameLoopPhysicsFeature;
        private InputFeature _inputFeature;

        public GameOverState
        (
            IWindowService windowService,
            IGameStateMachine gameStateMachine,
            ISystemFactory systemFactory,
            GameContext gameContext,
            InputContext inputContext
        )
        {
            _gameStateMachine = gameStateMachine;
            _systemFactory = systemFactory;
            _gameContext = gameContext;
            _inputContext = inputContext;
            _windowService = windowService;
        }

        public override void Enter()
        {
            _gameLoopFeature = _systemFactory.Create<GameLoopFeature>();
            _gameLoopPhysicsFeature = _systemFactory.Create<GameLoopPhysicsFeature>();
            _inputFeature = _systemFactory.Create<InputFeature>();
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

            _gameLoopFeature.DeactivateReactiveSystems();
            _gameLoopPhysicsFeature.DeactivateReactiveSystems();
            _inputFeature.DeactivateReactiveSystems();
            _inputFeature.ClearReactiveSystems();
            _gameLoopFeature.ClearReactiveSystems();
            _gameLoopPhysicsFeature.ClearReactiveSystems();

            foreach (GameEntity entity in _gameContext.GetEntities())
                entity.isDestructed = true;

            _inputFeature.Cleanup();
            _gameLoopFeature.Cleanup();
            _gameLoopPhysicsFeature.Cleanup();
            _gameLoopFeature.TearDown();
            _gameLoopPhysicsFeature.TearDown();
            _inputFeature.TearDown();
            
            _gameLoopFeature = null;
            _inputFeature = null;
            _gameLoopPhysicsFeature = null;

            foreach (GameEntity entity in _gameContext.GetEntities())
                entity.Destroy();

            foreach (InputEntity entity in _inputContext.GetEntities())
                entity.Destroy();
        }
    }
}