using Code.Gameplay.Features;
using Code.Gameplay.Input;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.Systems;
using Zenject;

namespace Code.Infrastructure.States.GameStates.Game
{
    public class BeginDayLoopState : EndOfFrameExitState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly ISystemFactory _systemFactory;
        private readonly IGameStateHandlerService _gameStateHandlerService;

        private BeginDayFeature _gameLoopFeature;
        private InputFeature _inputFeature;

        [Inject]
        public BeginDayLoopState
        (
            ISystemFactory systemFactory,
            IGameStateHandlerService gameStateHandlerService
        )
        {
            _gameStateHandlerService = gameStateHandlerService;
            _systemFactory = systemFactory;
        }

        public override void Enter()
        {
            base.Enter();

            _gameLoopFeature = _systemFactory.Create<BeginDayFeature>();
            _inputFeature = _systemFactory.Create<InputFeature>();

            _inputFeature.Initialize();
            _gameLoopFeature.Initialize();
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
            _inputFeature.DeactivateReactiveSystems();

            _inputFeature.ClearReactiveSystems();
            _gameLoopFeature.ClearReactiveSystems();
        }

        private void CleanUp()
        {
            _inputFeature.Cleanup();
            _gameLoopFeature.Cleanup();
        }

        private void TearDown()
        {
            _gameLoopFeature.TearDown();
            _inputFeature.TearDown();
        }

        private void Clear()
        {
            _gameLoopFeature = null;
            _inputFeature = null;
        }
    }
}