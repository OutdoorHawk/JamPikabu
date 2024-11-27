using Code.Common.Extensions;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Common;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Settings.Window
{
    public class SettingsWindow : BaseWindow
    {
        [SerializeField] private Button _giveUpButton;

        private IGameStateMachine _gameStateMachine;

        [Inject]
        private void Construct(IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (_gameStateMachine.ActiveState is GameLoopState)
            {
                Time.timeScale = 0;
                _giveUpButton.EnableElement();
                return;
            }

            _giveUpButton.DisableElement();
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            Time.timeScale = 1;
        }

        protected override void SubscribeUpdates()
        {
            base.SubscribeUpdates();
            _giveUpButton.onClick.AddListener(OpenConfirmWindow);
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            _giveUpButton.onClick.RemoveListener(OpenConfirmWindow);
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