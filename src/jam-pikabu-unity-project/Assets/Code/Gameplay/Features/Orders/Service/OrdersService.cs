using System;
using System.Collections.Generic;
using System.Linq;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Features.Orders.Factory;
using Code.Gameplay.StaticData;
using Code.Meta.Features.Days.Service;
using RoyalGold.Sources.Scripts.Game.MVC.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Gameplay.Features.Orders.Service
{
    public class OrdersService : IOrdersService
    {
        public event Action OnOrderUpdated;

        private readonly IStaticDataService _staticDataService;
        private readonly IDaysService _daysService;
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
        public int MaxOrders => _daysService.GetDayData().OrdersAmount;
        public bool OrderWindowSeen => _orderWindowSeen;

        public (List<IngredientData> good, List<IngredientData> bad) OrderIngredients => _orderIngredients;

        public OrdersService
        (
            IStaticDataService staticDataService,
            IDaysService daysService,
            IOrdersFactory ordersFactory,
            ILootService lootService
        )
        {
            _staticDataService = staticDataService;
            _daysService = daysService;
            _ordersFactory = ordersFactory;
            _lootService = lootService;
        }

        public void InitDayBegin()
        {
            _ordersData = _staticDataService.GetStaticData<OrdersStaticData>();
            _currentOrderIndex = 0;
            _ordersCompleted = 0;
            _ordersBuffer.Clear();

            InitCurrentDayOrders(_daysService.CurrentDay);
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
            return _ordersBuffer[_currentOrderIndex];
        }

        public bool CheckAllOrdersCompleted()
        {
            return _ordersCompleted + 1 >= _daysService.GetDayData().OrdersAmount;
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

            OrderSetup orderSetup = order.Setup;
            Vector2Int minMaxGoodIngredients = orderSetup.MinMaxGoodIngredients;
            Vector2Int minMaxBadIngredients = orderSetup.MinMaxBadIngredients;
            Vector2Int minMaxFactor = orderSetup.MinMaxIngredientsRatingFactor;

            int goodCount = Random.Range(minMaxGoodIngredients.x, minMaxGoodIngredients.y + 1);
            int badCount = Random.Range(minMaxBadIngredients.x, minMaxBadIngredients.y + 1);

            FillIngredientsRandom
            (
                availableLoot,
                goodCount,
                _orderIngredients.good,
                minMaxFactor,
                orderSetup.MinMaxAmount,
                IngredientTypeId.Good
            );

            FillIngredientsRandom
            (
                availableLoot,
                badCount,
                _orderIngredients.bad,
                minMaxFactor,
                orderSetup.MinMaxAmount,
                IngredientTypeId.Bad, 
                _orderIngredients.good
            );
        }

        private static void FillIngredientsRandom
        (
            List<LootSetup> availableLoot,
            int countToCreate,
            List<IngredientData> listToFill,
            Vector2Int minMaxRatingFactor,
            Vector2Int needAmountMinMax,
            IngredientTypeId ingredientType,
            List<IngredientData> excludeList = null
        )
        {
            foreach (var lootSetup in availableLoot)
            {
                if (countToCreate <= 0)
                    break;

                if (listToFill.Exists(data => data.TypeId == lootSetup.Type))
                    continue;

                if (excludeList != null && excludeList.Exists(data => data.TypeId == lootSetup.Type))
                    continue;

                int ratingFactor = Random.Range(minMaxRatingFactor.x, minMaxRatingFactor.y + 1);
                int amount = Random.Range(needAmountMinMax.x, needAmountMinMax.y + 1);

                var data = new IngredientData
                (
                    typeId: lootSetup.Type,
                    ingredientType: ingredientType,
                    ratingFactor: ratingFactor,
                    amount
                );
                
                listToFill.Add(data);
                countToCreate--;
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