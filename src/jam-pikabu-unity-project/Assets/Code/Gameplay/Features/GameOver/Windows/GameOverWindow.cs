using Code.Common.Extensions;
using Code.Gameplay.Features.GameOver.Service;
using Code.Gameplay.Features.RoundState.Service;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.Windows;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using TMPro;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.GameOver.Windows
{
    public class GameOverWindow : BaseWindow
    {
        private IGameStateMachine _gameStateMachine;
        private ISoundService _soundService;
        private IGameOverService _gameOverService;
        
        [SerializeField] private TMP_Text _gameOverText;
        [SerializeField] private TMP_Text _gameOverBossText;
        private IRoundStateService _roundStateService;

        [Inject]
        private void Construct(IGameStateMachine gameStateMachine, ISoundService soundService, IGameOverService gameOverService, IRoundStateService roundStateService
        )
        {
            _roundStateService = roundStateService;
            _gameOverService = gameOverService;
            _soundService = soundService;
            _gameStateMachine = gameStateMachine;
        }

        protected override void Initialize()
        {
            base.Initialize();
            PlaySound();
            CloseButton?.onClick.AddListener(Restart);

            if (_gameOverService.IsGameWin)
                return;

            if (_roundStateService.GetDayData().IsBoss)
            {
                _gameOverText.DisableElement();
                _gameOverBossText.EnableElement();
            }
            else
            {
                _gameOverText.EnableElement();
                _gameOverBossText.DisableElement();
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            CloseButton?.onClick.RemoveListener(Restart);
        }

        private void Restart()
        {
            _gameStateMachine.Enter<LoadLevelSimpleState, LoadLevelPayloadParameters>(new LoadLevelPayloadParameters());
        }

        private void PlaySound()
        {
            if (_gameOverService.IsGameWin)
            {
                _soundService.PlayOneShotSound(SoundTypeId.Level_Win);
            }
            else
            {
                _soundService.PlayOneShotSound(SoundTypeId.Level_Lost);
            }
        }
    }
}