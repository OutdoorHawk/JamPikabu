using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.StaticData;

namespace Code.Gameplay.Features.Currency.Service
{
    public class GameplayCurrencyService : IGameplayCurrencyService, IOnConfigsInitInitHandler
    {
        private readonly ISoundService _soundService;
        private readonly IStaticDataService _staticDataService;
        private readonly ICurrencyFactory _currencyFactory;
        
        public event Action CurrencyChanged;

        private int _currentTurnCostGold;

        private readonly Dictionary<CurrencyTypeId, CurrencyCount> _currencies = new();

        public int CurrentTurnCostGold => _currentTurnCostGold;

        public GameplayCurrencyService
        (
            ISoundService soundService,
            IStaticDataService staticDataService
        )
        {
            _soundService = soundService;
            _staticDataService = staticDataService;
        }

        public void OnConfigsInitInitComplete()
        {
            var currencyConfig = _staticDataService.GetStaticData<CurrencyStaticData>();

            foreach (CurrencyConfig config in currencyConfig.Configs)
                _currencies.Add(config.CurrencyTypeId, new CurrencyCount());
        }

        public int GetCurrencyOfType(CurrencyTypeId typeId)
        {
            CurrencyCount currency = GetCurrencyOfTypeInternal(typeId);
            if (currency == null)
                return 0;
            return currency.Amount - currency.Withdraw;
        }

        public void UpdateCurrencyAmount(int newAmount, CurrencyTypeId typeId)
        {
            CurrencyCount currency = GetCurrencyOfTypeInternal(typeId);

            if (currency.Amount != newAmount)
            {
                PlaySoftCurrencySound(newAmount, currency);
                currency.Amount = newAmount;
                CurrencyChanged?.Invoke();
            }
        }

        public void AddWithdraw(int amount, CurrencyTypeId typeId)
        {
            CurrencyCount currency = GetCurrencyOfTypeInternal(typeId);
            currency.Withdraw += amount;
            CurrencyChanged?.Invoke();
        }

        public void UpdateCurrentTurnCostAmount(int newAmount)
        {
            if (Math.Abs(newAmount - _currentTurnCostGold) > float.Epsilon)
            {
                _currentTurnCostGold = newAmount;
                CurrencyChanged?.Invoke();
            }
        }

        private void PlaySoftCurrencySound(int newAmount, CurrencyCount currency)
        {
            if (currency.Amount == 0)
                return;

            if (newAmount > currency.Amount)
                _soundService.PlaySound(SoundTypeId.Soft_Currency_Collect);
        }

        private CurrencyCount GetCurrencyOfTypeInternal(CurrencyTypeId typeId)
        {
            return _currencies[typeId];
        }

        public void Cleanup()
        {
            for (CurrencyTypeId i = 0; i < CurrencyTypeId.Count; i++)
            {
                _currencies[i] = default;
            }

            CurrencyChanged = null;
        }
    }
}