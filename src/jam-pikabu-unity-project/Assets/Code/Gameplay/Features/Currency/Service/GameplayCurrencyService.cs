using System;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using UnityEngine;

namespace Code.Gameplay.Features.Currency.Service
{
    public class GameplayCurrencyService : IGameplayCurrencyService
    {
        private readonly ISoundService _soundService;
        public event Action CurrencyChanged;
        
        private int _currentGold;
        private int _currentGoldWithdraw;
        private int _currentTurnCostGold;

        public CurrencyTypeId GoldCurrencyType => CurrencyTypeId.Gold;

        public int CurrentGoldCurrency => _currentGold - _currentGoldWithdraw;
        public int CurrentTurnCostGold => _currentTurnCostGold;

        public GameplayCurrencyService(ISoundService soundService)
        {
            _soundService = soundService;
        }

        public void AddWithdraw(int amount)
        {
            _currentGoldWithdraw += amount;
            CurrencyChanged?.Invoke();
            Debug.LogError($"_currentGoldWithdraw {_currentGoldWithdraw} ");
        }
        
        public void UpdateCurrentGoldAmount(int newAmount)
        {
            if (_currentGold != newAmount )
            {
                PlaySoftCurrencySound(newAmount);
                _currentGold = newAmount;
                CurrencyChanged?.Invoke();
            }
        }
        
        public void UpdateCurrentTurnCostAmount(int newAmount)
        {
            if (Math.Abs(newAmount - _currentTurnCostGold) > float.Epsilon)
            {
                _currentTurnCostGold = newAmount;
                CurrencyChanged?.Invoke();
            }
        }

        private void PlaySoftCurrencySound(int newAmount)
        {
            if (_currentGold == 0)
                return;
            
            if (newAmount > _currentGold) 
                _soundService.PlaySound(SoundTypeId.Soft_Currency_Collect);
        }

        public void Cleanup()
        {
            _currentGold = 0;
            CurrencyChanged = null;
        }
    }
}