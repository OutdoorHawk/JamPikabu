using Code.Gameplay.Features.RoundState.Service;
using Code.Gameplay.Input.Service;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;
using Code.Progress.Data;
using Code.Progress.Provider;
using Code.Progress.SaveLoadService;

namespace Code.Gameplay.Features.GameOver.Service
{
    public class GameOverService : IGameOverService, IEnterGameLoopStateHandler
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IProgressProvider _progressProvider;
        private readonly IInputService _inputService;
        private readonly IWindowService _windowService;
        private readonly IStaticDataService _staticData;
        private readonly IRoundStateService _roundStateService;

        public bool IsGameWin { get; private set; }

        public GameOverService
        (
            IGameStateMachine gameStateMachine,
            ISaveLoadService saveLoadService,
            IProgressProvider progressProvider,
            IInputService inputService,
            IWindowService windowService,
            IStaticDataService staticData,
            IRoundStateService roundStateService
        )
        {
            _windowService = windowService;
            _staticData = staticData;
            _roundStateService = roundStateService;
            _inputService = inputService;
            _gameStateMachine = gameStateMachine;
            _saveLoadService = saveLoadService;
            _progressProvider = progressProvider;
        }

        public OrderType OrderType => OrderType.First;

        public void OnEnterGameLoop()
        {
            IsGameWin = false;
        }

        public void GameWin()
        {
            if (_gameStateMachine.ActiveState is GameOverState)
                return;

            IsGameWin = true;
            SaveLevelPassedProgress();
            BlockInput();
            _gameStateMachine.Enter<GameOverState>();
        }

        public void GameOver()
        {
            if (_gameStateMachine.ActiveState is GameOverState)
                return;

            BlockInput();
            _gameStateMachine.Enter<GameOverState>();
            _windowService.OpenWindow(WindowTypeId.GameLostWindow);
            _saveLoadService.SaveProgress();
            _roundStateService.ResetCurrentRound();
        }

        private void BlockInput()
        {
            _inputService.DisableAllInput();
        }

        private void SaveLevelPassedProgress()
        {
        }
    }
}