using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.StaticData;
using Code.Infrastructure.States.GameStates.Game;
using Code.Infrastructure.States.StateMachine;
using Code.Meta.Features.Days.Configs;

namespace Code.Gameplay.Features.Currency.Service
{
    public class GameplayCurrencyService : IGameplayCurrencyService, IConfigsInitHandler
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ICurrencyFactory _currencyFactory;

        public event Action CurrencyChanged;

        private int _currentTurnCostGold;

        private readonly Dictionary<CurrencyTypeId, CurrencyCount> _currencies = new();

        public GameplayCurrencyService
        (
            IStaticDataService staticDataService,
            IGameStateMachine gameStateMachine
        )
        {
            _staticDataService = staticDataService;
            _gameStateMachine = gameStateMachine;
        }

        public void OnConfigsInitInitComplete()
        {
            InitCurrency();
        }

        public int GetCurrencyOfType(CurrencyTypeId typeId)
        {
            CurrencyCount currency = GetCurrencyOfTypeInternal(typeId);

            if (currency == null)
                return 0;
            return currency.Amount - currency.Withdraw;
        }

        public void UpdateCurrencyAmount(int newAmount, int withdraw, CurrencyTypeId typeId)
        {
            if (_gameStateMachine.ActiveState is GameOverState)
                return;

            CurrencyCount currency = GetCurrencyOfTypeInternal(typeId);
            
            if (currency == null)
                return;
            
            bool changed = false;

            if (currency.Withdraw != withdraw)
            {
                currency.Withdraw = withdraw;
                changed = true;
            }

            if (currency.Amount != newAmount)
            {
                currency.Amount = newAmount;
                changed = true;
            }

            if (changed)
                CurrencyChanged?.Invoke();
        }

        private CurrencyCount GetCurrencyOfTypeInternal(CurrencyTypeId typeId)
        {
            return _currencies.GetValueOrDefault(typeId);
        }

        public void Cleanup()
        {
            _currencies.Clear();
            CurrencyChanged = null;
        }

        public void InitCurrency()
        {
            var currencyConfig = _staticDataService.GetStaticData<CurrencyStaticData>();
            var roundState = _staticDataService.GetStaticData<DaysStaticData>();

            foreach (CurrencyConfig config in currencyConfig.Configs)
                _currencies.Add(config.CurrencyTypeId, new CurrencyCount());

            _currencies[CurrencyTypeId.Gold].Amount = roundState.StartGoldAmount;
        }
    }
}