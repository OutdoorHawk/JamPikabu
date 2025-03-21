using System.Collections.Generic;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Handler;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Result.Window;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using Code.Meta.Features.Days.Service;
using Cysharp.Threading.Tasks;

namespace Code.Gameplay.Features.Result.Service
{
    public class ResultWindowService : IResultWindowService,
        IEnterGameLoopStateHandler,
        IExitGameLoopStateHandler,
        IGameplayCurrencyChangedHandler
    {
        private readonly IWindowService _windowService;
        private readonly IDaysService _daysService;

        private readonly Dictionary<LootTypeId, int> _collectedLoot = new();
        private readonly Dictionary<CurrencyTypeId, int> _initialCurrency = new();
        private readonly Dictionary<CurrencyTypeId, int> _collectedCurrency = new();

        public int CurrentDay { get; private set; }
        public OrderType StateHandlerOrder => OrderType.Last;

        public ResultWindowService
        (
            IWindowService windowService,
            IDaysService daysService
        )
        {
            _windowService = windowService;
            _daysService = daysService;
        }

        public void OnEnterGameLoop()
        {
            _daysService.OnDayBegin += CacheDayData;
        }

        public void OnExitGameLoop()
        {
            _daysService.OnDayBegin -= CacheDayData;
            Cleanup();
        }

        private void CacheDayData()
        {
            CurrentDay = _daysService.CurrentDay;
        }

        public void OnCurrencyChanged(CurrencyTypeId type, int newAmount)
        {
            UpdateStats(type, newAmount);
        }

        public void InitInitialCurrency(CurrencyTypeId type, int amount)
        {
            _initialCurrency[type] = amount;
        }

        public async UniTask TryShowProfitWindow()
        {
            var window = await _windowService.OpenWindow<ResultWindow>(WindowTypeId.ResultWindow);
            await UniTask.WaitWhile(() => _windowService.IsWindowOpen(WindowTypeId.ResultWindow), cancellationToken: window.destroyCancellationToken);
        }

        public void AddCollectedLoot(LootTypeId lootTypeId)
        {
            if (_collectedLoot.TryGetValue(lootTypeId, out int count))
            {
                _collectedLoot[lootTypeId] = count + 1;
                return;
            }

            _collectedLoot[lootTypeId] = 1;
        }

        public IReadOnlyDictionary<LootTypeId, int> GetCollectedLoot()
        {
            return _collectedLoot;
        }

        public int GetCollectedCurrency(CurrencyTypeId currencyType)
        {
            return _collectedCurrency.GetValueOrDefault(currencyType, 0) - _initialCurrency.GetValueOrDefault(currencyType);
        }

        public int GetTotalRating()
        {
            int plus = _collectedCurrency.GetValueOrDefault(CurrencyTypeId.Plus, 0);
            int minus = _collectedCurrency.GetValueOrDefault(CurrencyTypeId.Minus, 0);
            return plus - minus;
        }

        public bool CheckGameWin()
        {
            return GetTotalRating() >= _daysService.DayStarsData[0].RatingAmountNeed;
        }

        private void UpdateStats(CurrencyTypeId type, int newAmount)
        {
            switch (type)
            {
                case CurrencyTypeId.Gold:
                case CurrencyTypeId.Plus:
                case CurrencyTypeId.Minus:
                    _collectedCurrency[type] = newAmount;
                    break;
            }
        }

        private void Cleanup()
        {
            _initialCurrency.Clear();
            _collectedLoot.Clear();
            _collectedCurrency.Clear();
        }
    }
}