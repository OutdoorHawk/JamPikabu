using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Currency.Behaviours;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Currency.Handler;
using Code.Gameplay.StaticData;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using Code.Infrastructure.States.GameStates.Game;
using Code.Infrastructure.States.StateMachine;
using Zenject;

namespace Code.Gameplay.Features.Currency.Service
{
    public class GameplayCurrencyService : IGameplayCurrencyService, ILoadProgressStateHandler
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IStaticDataService _staticDataService;
        private readonly LazyInject<List<IGameplayCurrencyChangedHandler>> _handlers;

        public event Action CurrencyChanged;

        public CurrencyHolder Holder { get; private set; }

        private readonly Dictionary<CurrencyTypeId, CurrencyCount> _currencies = new();

        public OrderType StateHandlerOrder => OrderType.Last;

        public IReadOnlyDictionary<CurrencyTypeId, CurrencyCount> Currencies => _currencies;

        public GameplayCurrencyService
        (
            IGameStateMachine gameStateMachine,
            IStaticDataService staticDataService,
            LazyInject<List<IGameplayCurrencyChangedHandler>> handlers
        )
        {
            _gameStateMachine = gameStateMachine;
            _staticDataService = staticDataService;
            _handlers = handlers;
        }

        public void RegisterHolder(CurrencyHolder currencyHolder)
        {
            Holder = currencyHolder;
        }

        public void UnregisterHolder(CurrencyHolder currencyHolder)
        {
            Holder = null;
        }

        public void OnEnterLoadProgress()
        {
            
        }

        public void OnExitLoadProgress()
        {
            InitCurrency();
        }

        public int GetCurrencyOfType(CurrencyTypeId typeId, bool applyWithdraw = true)
        {
            CurrencyCount currency = GetCurrencyOfTypeInternal(typeId);

            if (currency == null)
                return 0;

            if (applyWithdraw)
                return currency.Amount - currency.Withdraw;
            else
                return currency.Amount;
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
                NotifyCurrencyChanged(typeId, newAmount);
        }

        private CurrencyCount GetCurrencyOfTypeInternal(CurrencyTypeId typeId)
        {
            return _currencies.GetValueOrDefault(typeId);
        }

        private void NotifyCurrencyChanged(CurrencyTypeId typeId, int newAmount)
        {
            foreach (var handler in _handlers.Value)
                handler.OnCurrencyChanged(typeId, newAmount);

            CurrencyChanged?.Invoke();
        }

        private void InitCurrency()
        {
            var currencyConfig = _staticDataService.Get<CurrencyStaticData>();

            foreach (CurrencyConfig config in currencyConfig.Configs)
                _currencies[config.CurrencyTypeId] = new CurrencyCount();
        }
    }
}