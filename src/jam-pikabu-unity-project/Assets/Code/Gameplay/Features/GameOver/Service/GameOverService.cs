using System.Threading;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Input.Service;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.GameStates.Game;
using Code.Infrastructure.States.StateMachine;
using Code.Progress.Provider;
using Code.Progress.SaveLoadService;
using Cysharp.Threading.Tasks;
using static Code.Common.Extensions.AsyncGameplayExtensions;

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
        private readonly IOrdersService _ordersService;
        private readonly IGameplayCurrencyService _gameplayCurrencyService;

        public bool IsGameWin { get; private set; }

        public GameOverService
        (
            IGameStateMachine gameStateMachine,
            ISaveLoadService saveLoadService,
            IProgressProvider progressProvider,
            IInputService inputService,
            IWindowService windowService,
            IStaticDataService staticData,
            IOrdersService ordersService,
            IGameplayCurrencyService gameplayCurrencyService
        )
        {
            _windowService = windowService;
            _staticData = staticData;
            _ordersService = ordersService;
            _gameplayCurrencyService = gameplayCurrencyService;
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

            GameOverAsync().Forget();
        }

        private async UniTaskVoid GameOverAsync()
        {
            BlockInput();
            _gameStateMachine.Enter<GameOverState>();
            _saveLoadService.SaveProgress();
            _ordersService.GameOver();
            _gameplayCurrencyService.Cleanup();
            await DelaySeconds(1, new CancellationToken());
            _windowService.OpenWindow(WindowTypeId.GameLostWindow);
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