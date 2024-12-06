using Code.Gameplay.Features;
using Code.Gameplay.Input;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.Systems;
using Zenject;

namespace Code.Infrastructure.States.GameStates.Game
{
    public class RoundCompletionLoopState : EndOfFrameExitState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly ISystemFactory _systemFactory;
        private readonly IGameStateHandlerService _gameStateHandlerService;

        private RoundCompletionFeature _roundLoopFeature;
        private InputFeature _inputFeature;

        [Inject]
        public RoundCompletionLoopState
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

            _roundLoopFeature = _systemFactory.Create<RoundCompletionFeature>();
            _inputFeature = _systemFactory.Create<InputFeature>();

            _inputFeature.Initialize();
            _roundLoopFeature.Initialize();
        }

        protected override void OnUpdate()
        {
            _inputFeature.Execute();
            _inputFeature.Cleanup();

            _roundLoopFeature.Execute();
            _roundLoopFeature.Cleanup();
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
            _roundLoopFeature.DeactivateReactiveSystems();
            _inputFeature.DeactivateReactiveSystems();

            _inputFeature.ClearReactiveSystems();
            _roundLoopFeature.ClearReactiveSystems();
        }

        private void CleanUp()
        {
            _inputFeature.Cleanup();
            _roundLoopFeature.Cleanup();
        }

        private void TearDown()
        {
            _roundLoopFeature.TearDown();
            _inputFeature.TearDown();
        }

        private void Clear()
        {
            _roundLoopFeature = null;
            _inputFeature = null;
        }
    }
}