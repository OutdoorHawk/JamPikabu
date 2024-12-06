using Code.Gameplay.Features;
using Code.Gameplay.Input;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.Systems;

namespace Code.Infrastructure.States.GameStates.Game
{
    public class GameOverState : EndOfFrameExitState
    {
        private readonly ISystemFactory _systemFactory;
        private readonly GameContext _gameContext;
        private readonly InputContext _inputContext;

        private RoundPreparationLoopFeature _gameLoopFeature;
        private InputFeature _inputFeature;

        public GameOverState
        (
            ISystemFactory systemFactory,
            GameContext gameContext,
            InputContext inputContext
        )
        {
            _systemFactory = systemFactory;
            _gameContext = gameContext;
            _inputContext = inputContext;
        }

        public override void Enter()
        {
            _gameLoopFeature = _systemFactory.Create<RoundPreparationLoopFeature>();
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
            _inputFeature.DeactivateReactiveSystems();
            _inputFeature.ClearReactiveSystems();
            _gameLoopFeature.ClearReactiveSystems();

            foreach (GameEntity entity in _gameContext.GetEntities())
                entity.isDestructed = true;

            _inputFeature.Cleanup();
            _gameLoopFeature.Cleanup();
            _gameLoopFeature.TearDown();
            _inputFeature.TearDown();

            _gameLoopFeature = null;
            _inputFeature = null;

            foreach (GameEntity entity in _gameContext.GetEntities())
                entity.Destroy();

            foreach (InputEntity entity in _inputContext.GetEntities())
                entity.Destroy();
        }
    }
}