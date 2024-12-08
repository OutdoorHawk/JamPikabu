using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Common.MousePosition;
using Code.Gameplay.Features.RoundState.Configs;
using Code.Gameplay.StaticData;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.Systems;

namespace Code.Infrastructure.States.GameStates.Game
{
    public class GameEnterState : SimpleState
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly ISystemFactory _systems;
        private readonly GameContext _gameContext;
        private readonly IMousePositionService _mousePositionService;
        private readonly IGameStateHandlerService _gameStateHandlerService;
        private readonly IStaticDataService _staticDataService;

        public GameEnterState
        (
            IGameStateMachine stateMachine,
            IMousePositionService mousePositionService,
            IGameStateHandlerService gameStateHandlerService,
            IStaticDataService staticDataService
        )
        {
            _gameStateHandlerService = gameStateHandlerService;
            _staticDataService = staticDataService;
            _mousePositionService = mousePositionService;
            _stateMachine = stateMachine;
        }

        public override void Enter()
        {
            _mousePositionService.Initialize();
            _gameStateHandlerService.OnEnterGameLoop();
            _stateMachine.Enter<GameLoopState>();
          
        }
    }
}