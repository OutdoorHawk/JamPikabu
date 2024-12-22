using Code.Gameplay.Features;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Input;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.Systems;

namespace Code.Infrastructure.States.GameStates.Game
{
    public class GameOverState : EndOfFrameExitState
    {
        private readonly ISystemFactory _systemFactory;
        private readonly GameContext _gameContext;
        private readonly InputContext _inputContext;
        private readonly IGameplayCurrencyService _gameplayCurrencyService;
        private readonly IGameStateHandlerService _gameStateHandlerService;

        private GameLoopFeature _gameLoopFeature; 
        private GameLoopPhysicsFeature _physicsLoopFeature;
        private InputFeature _inputFeature;

        public GameOverState
        (
            ISystemFactory systemFactory,
            GameContext gameContext,
            InputContext inputContext,
            IGameplayCurrencyService gameplayCurrencyService,
            IGameStateHandlerService gameStateHandlerService
        )
        {
            _systemFactory = systemFactory;
            _gameContext = gameContext;
            _inputContext = inputContext;
            _gameplayCurrencyService = gameplayCurrencyService;
            _gameStateHandlerService = gameStateHandlerService;
        }

        public override void Enter()
        {
            _gameLoopFeature = _systemFactory.Create<GameLoopFeature>();
            _physicsLoopFeature = _systemFactory.Create<GameLoopPhysicsFeature>();
            _inputFeature = _systemFactory.Create<InputFeature>();
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
            _physicsLoopFeature.DeactivateReactiveSystems();
            _inputFeature.DeactivateReactiveSystems();
            _inputFeature.ClearReactiveSystems();
            _gameLoopFeature.ClearReactiveSystems();
            _physicsLoopFeature.ClearReactiveSystems();

            foreach (GameEntity entity in _gameContext.GetEntities())
                entity.isDestructed = true;

            _inputFeature.Cleanup();
            _gameLoopFeature.Cleanup();
            _physicsLoopFeature.Cleanup();
            _gameLoopFeature.TearDown();
            _physicsLoopFeature.TearDown();
            _inputFeature.TearDown();

            _gameLoopFeature = null;
            _physicsLoopFeature = null;
            _inputFeature = null;

            foreach (GameEntity entity in _gameContext.GetEntities())
                entity.Destroy();

            foreach (InputEntity entity in _inputContext.GetEntities())
                entity.Destroy();
            
            _gameStateHandlerService.OnExitGameLoop();
        }
    }
}