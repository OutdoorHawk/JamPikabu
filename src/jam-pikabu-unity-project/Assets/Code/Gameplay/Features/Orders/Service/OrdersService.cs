using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Features.Orders.Factory;
using Code.Gameplay.Features.RoundState.Service;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using RoyalGold.Sources.Scripts.Game.MVC.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Gameplay.Features.Orders.Service
{
    public class OrdersService : IOrdersService
    {
        public event Action OnOrderUpdated;

        private readonly IStaticDataService _staticDataService;
        private readonly IRoundStateService _roundStateService;
        private readonly IOrdersFactory _ordersFactory;
        private readonly ILootService _lootService;
        private readonly IWindowService _windowService;

        private OrdersStaticData _ordersData;
        private int _currentOrderIndex;
        private int _ordersCompleted;

        private (List<IngredientData> good, List<IngredientData> bad) _orderIngredients;
        private readonly List<OrderData> _ordersBuffer = new();
        private readonly Dictionary<LootTypeId, IngredientData> _orderIngredientCostDict = new();

        public int OrdersCompleted => _ordersCompleted;
        public int MaxOrders => _ordersData.OrdersAmountInDay;

        public (List<IngredientData> good, List<IngredientData> bad) OrderIngredients => _orderIngredients;

        public OrdersService(IStaticDataService staticDataService,
            IRoundStateService roundStateService, IOrdersFactory ordersFactory, 
            ILootService lootService, IWindowService windowService)
        {
            _staticDataService = staticDataService;
            _roundStateService = roundStateService;
            _ordersFactory = ordersFactory;
            _lootService = lootService;
            _windowService = windowService;
        }

        public void InitDay(int currentDay)
        {
            _ordersData = _staticDataService.GetStaticData<OrdersStaticData>();
            _currentOrderIndex = 0;
            _ordersCompleted = 0;
            _ordersBuffer.Clear();

            InitCurrentDayOrders(currentDay);
        }

        private void InitCurrentDayOrders(int currentDay)
        {
            List<OrderData> ordersData = _ordersData.Configs;

            foreach (var data in ordersData)
            {
                if (CheckMinDayToUnlock(data, currentDay))
                    continue;

                if (CheckMaxDayToUnlock(data, currentDay))
                    continue;

                _ordersBuffer.Add(data);
            }

            _ordersBuffer.ShuffleList();
        }

        public GameEntity CreateOrder()
        {
            var order = GetCurrentOrder();
            InitIngredientsDic(order);
            OnOrderUpdated?.Invoke();
            return _ordersFactory.CreateOrder(order);
        }

        public void AddIngredientTypedData(LootTypeId lootLootTypeId, IngredientData ingredientData)
        {
            _orderIngredientCostDict.Add(lootLootTypeId, ingredientData);
        }

        public IngredientData GetIngredientData(LootTypeId lootTypeId)
        {
            return _orderIngredientCostDict.GetValueOrDefault(lootTypeId);
        }

        public bool TryGetIngredientData(LootTypeId lootTypeId, out IngredientData ingredientData)
        {
            return _orderIngredientCostDict.TryGetValue(lootTypeId, out ingredientData);
        }

        public OrderData GetCurrentOrder()
        {
            return _ordersBuffer[_currentOrderIndex];
        }

        public bool CheckOrdersCompleted()
        {
            return _ordersCompleted + 1 >= _ordersData.OrdersAmountInDay;
        }

        public void GoToNextOrder()
        {
            _lootService.ClearCollectedLoot();
            _ordersCompleted++;

            _currentOrderIndex++;

            if (_currentOrderIndex >= _ordersBuffer.Count)
                _currentOrderIndex = 0;

            _roundStateService.PrepareToNextRound();
            OnOrderUpdated?.Invoke();
        }

        public void GameOver()
        {
            _currentOrderIndex = 0;
            _ordersBuffer.Clear();
        }

        private void InitIngredientsDic(OrderData order)
        {
            GetIngredientsFromOrder(order);
            _orderIngredientCostDict.Clear();
            FillIngredients(_orderIngredients.good);
            FillIngredients(_orderIngredients.bad);
        }

        private void GetIngredientsFromOrder(OrderData order)
        {
            if (order.Setup.RandomSetupEnabled == false)
            {
                _orderIngredients.good = new List<IngredientData>(order.Setup.GoodIngredients);
                _orderIngredients.bad = new List<IngredientData>(order.Setup.BadIngredients);
                return;
            }

            _orderIngredients.good = new List<IngredientData>();
            _orderIngredients.bad = new List<IngredientData>();
            List<LootSetup> availableLoot = new List<LootSetup>(_lootService.AvailableLoot);

            availableLoot.ShuffleList();

            Vector2Int minMaxGoodIngredients = order.Setup.MinMaxGoodIngredients;
            Vector2Int minMaxGoodReward = order.Setup.MinMaxGoodIngredientsReward;
            int goodCount = Random.Range(minMaxGoodIngredients.x, minMaxGoodIngredients.y + 1);

            Vector2Int minMaxBadIngredients = order.Setup.MinMaxBadIngredients;
            Vector2Int minMaxBadReward = order.Setup.MinMaxBadIngredientsReward;
            int badCount = Random.Range(minMaxBadIngredients.x, minMaxBadIngredients.y + 1);

            FillIngredientsRandom(availableLoot, goodCount, _orderIngredients.good, minMaxGoodReward, CurrencyTypeId.Plus);
            FillIngredientsRandom(availableLoot, badCount, _orderIngredients.bad, minMaxBadReward, CurrencyTypeId.Minus, _orderIngredients.good);
        }

        private static void FillIngredientsRandom(List<LootSetup> availableLoot, int goodCount, List<IngredientData> list, Vector2Int reward,
            CurrencyTypeId typeId, List<IngredientData> excludeList = null)
        {
            foreach (var lootSetup in availableLoot)
            {
                if (goodCount <= 0)
                    break;

                if (list.Exists(data => data.TypeId == lootSetup.Type))
                    continue;

                if (excludeList != null && excludeList.Exists(data => data.TypeId == lootSetup.Type))
                    continue;

                int rewardAmount = Random.Range(reward.x, reward.y + 1);
                var rewardMinMax = new CostSetup(typeId) { Amount = rewardAmount };
                list.Add(new IngredientData { TypeId = lootSetup.Type, Rating = rewardMinMax });
                goodCount--;
            }
        }

        private void FillIngredients(List<IngredientData> ingredients)
        {
            foreach (IngredientData ingredientData in ingredients)
            {
                if (_orderIngredientCostDict.TryAdd(ingredientData.TypeId, ingredientData) == false)
                    Debug.LogError($"Config error! Order cannot have same ingredient in bad and good ingredient list: {ingredientData}.");
            }
        }

        private static bool CheckMinDayToUnlock(OrderData data, int currentDay)
        {
            return data.Setup.MinDayToUnlock > 0 && currentDay < data.Setup.MinDayToUnlock;
        }

        private static bool CheckMaxDayToUnlock(OrderData data, int currentDay)
        {
            return data.Setup.MaxDayToUnlock > 0 && currentDay > data.Setup.MaxDayToUnlock;
        }
    }
}