using System;
using System.Collections.Generic;
using System.Linq;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Features.Orders.Factory;
using Code.Gameplay.Features.RoundState.Service;
using Code.Gameplay.StaticData;
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

        private OrdersStaticData _ordersData;
        private int _currentOrderIndex;
        private int _ordersCompleted;
        private bool _orderWindowSeen;

        private (List<IngredientData> good, List<IngredientData> bad) _orderIngredients;
        private readonly Dictionary<LootTypeId, IngredientData> _orderIngredientCostDict = new();
        private readonly List<OrderData> _ordersBuffer = new();

        public int OrdersCompleted => _ordersCompleted;
        public int MaxOrders => _roundStateService.GetDayData().OrdersAmount;
        public bool OrderWindowSeen => _orderWindowSeen;

        public (List<IngredientData> good, List<IngredientData> bad) OrderIngredients => _orderIngredients;

        public OrdersService
        (
            IStaticDataService staticDataService,
            IRoundStateService roundStateService,
            IOrdersFactory ordersFactory,
            ILootService lootService
        )
        {
            _staticDataService = staticDataService;
            _roundStateService = roundStateService;
            _ordersFactory = ordersFactory;
            _lootService = lootService;
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
            _orderWindowSeen = false;
            var order = GetCurrentOrder();
            InitIngredientsDic(order);
            OnOrderUpdated?.Invoke();
            return _ordersFactory.CreateOrder(order);
        }

        public void SetOrderWindowSeen()
        {
            _orderWindowSeen = true;
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
            if (_roundStateService.GetDayData().IsBoss)
                return _ordersBuffer.Find(data => data.Setup.IsBoss);

            return _ordersBuffer[_currentOrderIndex];
        }

        public bool CheckAllOrdersCompleted()
        {
            return _ordersCompleted + 1 >= _roundStateService.GetDayData().OrdersAmount;
        }

        public void GoToNextOrder()
        {
            _ordersCompleted++;

            _currentOrderIndex++;

            if (_currentOrderIndex >= _ordersBuffer.Count)
                _currentOrderIndex = 0;
        }

        public void GameOver()
        {
            _currentOrderIndex = 0;
            _ordersBuffer.Clear();
        }

        public bool OrderPassesConditions()
        {
            OrderData order = GetCurrentOrder();

            if (_lootService.CollectedLootItems.Count == 0)
                return false;

            if (CheckGoodMinimum(order) == false)
                return false;

            if (CheckBadMaximum(order) == false)
                return false;

            return true;
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

        private bool CheckGoodMinimum(OrderData order)
        {
            int setupCount = order.Setup.GoodMinimum;
            bool checkEnabled = setupCount > 0;

            if (checkEnabled == false)
                return true;

            int count = 0;
            foreach (var ingredient in _orderIngredients.good)
            {
                IEnumerable<LootTypeId> collectedOfType = _lootService.CollectedLootItems.Where(item => item == ingredient.TypeId);
                count += collectedOfType.Count();
            }

            return count >= setupCount;
        }

        private bool CheckBadMaximum(OrderData order)
        {
            int setupCount = order.Setup.BadMaximum;
            bool checkEnabled = setupCount > 0;

            if (checkEnabled == false)
                return true;

            int count = 0;
            foreach (var ingredient in _orderIngredients.bad)
            {
                IEnumerable<LootTypeId> collectedOfType = _lootService.CollectedLootItems.Where(item => item == ingredient.TypeId);
                count += collectedOfType.Count();
            }

            return count < setupCount;
        }

        private static bool CheckMinDayToUnlock(OrderData data, int currentDay)
        {
            return data.Setup.MinMaxDayToUnlock.x > 0 && currentDay < data.Setup.MinMaxDayToUnlock.x;
        }

        private static bool CheckMaxDayToUnlock(OrderData data, int currentDay)
        {
            return data.Setup.MinMaxDayToUnlock.y > 0 && currentDay > data.Setup.MinMaxDayToUnlock.y;
        }
    }
}