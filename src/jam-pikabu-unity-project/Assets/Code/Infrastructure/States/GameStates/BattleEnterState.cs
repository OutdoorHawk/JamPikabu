using Code.Gameplay.Cameras.Provider;
using Code.Gameplay.Common.MousePosition;
using Code.Infrastructure.SceneContext;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.Systems;

namespace Code.Infrastructure.States.GameStates
{
    public class BattleEnterState : SimpleState
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly ISceneContextProvider _sceneContextProvider;
        private readonly ISystemFactory _systems;
        private readonly GameContext _gameContext;
        private readonly IMousePositionService _mousePositionService;
        private readonly ICameraProvider _cameraProvider;

        public BattleEnterState
        (
            IGameStateMachine stateMachine,
            ISceneContextProvider sceneContextProvider,
            IMousePositionService mousePositionService,
            ICameraProvider cameraProvider
        )
        {
            _cameraProvider = cameraProvider;
            _mousePositionService = mousePositionService;
            _stateMachine = stateMachine;
            _sceneContextProvider = sceneContextProvider;
        }

        public override void Enter()
        {
            _mousePositionService.Initialize();
            _stateMachine.Enter<GameLoopState>();
        }
    }
}