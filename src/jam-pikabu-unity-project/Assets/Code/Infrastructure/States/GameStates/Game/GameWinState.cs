using Code.Gameplay.Windows.Service;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Progress.Provider;
using Code.Progress.SaveLoadService;

namespace Code.Infrastructure.States.GameStates.Game
{
    public class GameWinState : SimpleState
    {
        private readonly IWindowService _windowService;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IProgressProvider _progressProvider;

        public GameWinState
        (
            IWindowService windowService,
            IGameStateMachine gameStateMachine,
            ISaveLoadService saveLoadService,
            IProgressProvider progressProvider
        )
        {
            _progressProvider = progressProvider;
            _saveLoadService = saveLoadService;
            _gameStateMachine = gameStateMachine;
            _windowService = windowService;
        }

        public override void Enter()
        {
            _gameStateMachine.Enter<LoadMapMenuState>();
        }
    }
}