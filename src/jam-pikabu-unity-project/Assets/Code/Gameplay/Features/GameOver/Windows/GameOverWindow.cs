using Code.Common.Extensions;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Features.GameOver.Service;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.Windows;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Meta.Features.Days.Service;
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
        [SerializeField] private TMP_Text _ratingText;
        
        private IDaysService _daysService;
        private IGameplayCurrencyService _gameplayCurrencyService;

        [Inject]
        private void Construct(IGameStateMachine gameStateMachine, ISoundService soundService, 
            IGameOverService gameOverService, IDaysService daysService, IGameplayCurrencyService gameplayCurrencyService
        )
        {
            _gameplayCurrencyService = gameplayCurrencyService;
            _daysService = daysService;
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
            {
                InitRating();
                return;
            }

            if (_daysService.GetDayData().IsBossDay)
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
              
            }
            else
            {
                _soundService.PlayOneShotSound(SoundTypeId.Level_Lost);
            }
        }

        private void InitRating()
        {
            _ratingText.EnableElement();
            _ratingText.text = $"Ваши блюда набрали <color=green>{_gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Plus)}</color> плюсов и " +
                               $"<color=red>{_gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Minus)}</color> минусов";
        }
    }
}