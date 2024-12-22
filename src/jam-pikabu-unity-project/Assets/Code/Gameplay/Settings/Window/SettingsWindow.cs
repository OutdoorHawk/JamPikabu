using Code.Gameplay.Common.Time;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Common;
using Code.Infrastructure.States.GameStates.Game;
using Code.Infrastructure.States.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Settings.Window
{
    public class SettingsWindow : BaseWindow
    {
        private IGameStateMachine _gameStateMachine;
        private ITimeService _timeService;

        [Inject]
        private void Construct(IGameStateMachine gameStateMachine, ITimeService timeService)
        {
            _timeService = timeService;
            _gameStateMachine = gameStateMachine;
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (_gameStateMachine.ActiveState is GameLoopState or GameOverState)
                Time.timeScale = 0;
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            Time.timeScale = 1;
        }

        protected override void SubscribeUpdates()
        {
            base.SubscribeUpdates();
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
        }

        private void OpenConfirmWindow()
        {
            OpenGiveUp().Forget();
        }

        private async UniTaskVoid OpenGiveUp()
        {
            var window = await WindowService.OpenWindow<InfoWindow>(WindowTypeId.InfoWindow);
            window.SetActions(GameOver, () => WindowService.Close(WindowTypeId.InfoWindow));
        }

        private void GameOver()
        {
            WindowService.Close(WindowTypeId.InfoWindow);
            Close();
        }
    }
}