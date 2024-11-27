using Code.Gameplay.Features;
using Code.Gameplay.Input;
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
        
        private BattleFeature _battleFeature;
        private BattlePhysicsFeature _battlePhysicsFeature;
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
            // _windowService.Open(WindowId.GameOverWindow);
            
            _battleFeature = _systemFactory.Create<BattleFeature>();
            _battlePhysicsFeature = _systemFactory.Create<BattlePhysicsFeature>();
            _inputFeature = _systemFactory.Create<InputFeature>();
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

            _battleFeature.DeactivateReactiveSystems();
            _battlePhysicsFeature.DeactivateReactiveSystems();
            _inputFeature.DeactivateReactiveSystems();
            _inputFeature.ClearReactiveSystems();
            _battleFeature.ClearReactiveSystems();
            _battlePhysicsFeature.ClearReactiveSystems();

            foreach (GameEntity entity in _gameContext.GetEntities())
                entity.isDestructed = true;

            _inputFeature.Cleanup();
            _battleFeature.Cleanup();
            _battlePhysicsFeature.Cleanup();
            _battleFeature.TearDown();
            _battlePhysicsFeature.TearDown();
            _inputFeature.TearDown();
            
            _battleFeature = null;
            _inputFeature = null;
            _battlePhysicsFeature = null;

            foreach (GameEntity entity in _gameContext.GetEntities())
                entity.Destroy();

            foreach (InputEntity entity in _inputContext.GetEntities())
                entity.Destroy();
        }
    }
}