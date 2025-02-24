using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Handler;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Result.Window;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.Ads.Config;
using Code.Infrastructure.Ads.Service;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using Code.Meta.Features.Days.Service;
using Cysharp.Threading.Tasks;

namespace Code.Gameplay.Features.Result.Service
{
    public class ResultWindowService : IResultWindowService, 
        IExitGameLoopStateHandler, 
        IGameplayCurrencyChangedHandler
    {
        private readonly IAdsService _adsService;
        private readonly IWindowService _windowService;
        private readonly IStaticDataService _staticDataService;
        private readonly IDaysService _daysService;

        private readonly Dictionary<LootTypeId, int> _collectedLoot = new();
        private readonly Dictionary<CurrencyTypeId, int> _collectedCurrency = new();

        private AdsStaticData AdsStaticData => _staticDataService.Get<AdsStaticData>();

        public OrderType OrderType => OrderType.First;

        public ResultWindowService
        (
            IAdsService adsService,
            IWindowService windowService,
            IStaticDataService staticDataService,
            IDaysService daysService
        )
        {
            _adsService = adsService;
            _windowService = windowService;
            _staticDataService = staticDataService;
            _daysService = daysService;
        }

        public void OnExitGameLoop()
        {
            Cleanup();
        }

        public void OnCurrencyChanged(CurrencyTypeId type, int newAmount)
        {
            UpdateStats(type, newAmount);
        }

        public async UniTask TryShowProfitWindow()
        {
            if (_adsService.CanShowRewarded == false)
                return;

            if (_daysService.GetDaysProgress().Count < AdsStaticData.LevelsPassedToStartProfitAds)
                return;

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
            return _collectedCurrency.GetValueOrDefault(currencyType, 0);
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
            _collectedLoot.Clear();
            _collectedCurrency.Clear();
        }
    }
}