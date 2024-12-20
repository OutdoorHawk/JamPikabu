using System;
using System.Collections.Generic;
using System.Linq;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Features.Orders.Factory;
using Code.Gameplay.StaticData;
using Code.Meta.Features.Days.Configs;
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
        private readonly IGameplayLootService _gameplayLootService;

        private OrdersStaticData _ordersData;
        private int _currentOrderIndex;
        private int _ordersCompleted;

        private (List<IngredientData> good, List<IngredientData> bad) _orderIngredients;

        private readonly Dictionary<LootTypeId, IngredientData> _orderIngredientCostDict = new();
        private readonly List<OrderData> _ordersBuffer = new();

        public int OrdersCompleted => _ordersCompleted;
        public int MaxOrders => _daysService.GetDayData().OrdersAmount;

        public (List<IngredientData> good, List<IngredientData> bad) OrderIngredients => _orderIngredients;

        public OrdersService
        (
            IStaticDataService staticDataService,
            IDaysService daysService,
            IOrdersFactory ordersFactory,
            IGameplayLootService gameplayLootService
        )
        {
            _staticDataService = staticDataService;
            _daysService = daysService;
            _ordersFactory = ordersFactory;
            _gameplayLootService = gameplayLootService;
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

                if (CheckTag(data, currentDay) == false)
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

        public float GetOrderProgress()
        {
            (List<IngredientData> good, List<IngredientData> bad) = OrderIngredients;

            int total = good.Sum(data => data.Amount);
            int collected = 0;

            foreach (IngredientData ingredientData in good)
            {
                int collectedTypes = _gameplayLootService.CollectedLootItems.Count(type => type == ingredientData.TypeId);
                collectedTypes = Mathf.Clamp(collectedTypes, 0, ingredientData.Amount);
                collected += collectedTypes;
            }

            float progress = collected / (float)total;
            return progress;
        }

        public float GetPenaltyFactor()
        {
            (List<IngredientData> good, List<IngredientData> bad) = OrderIngredients;

            int total = good.Sum(data => data.Amount);

            float penaltyFactor = ApplyPenaltyFactor(bad, total);
            return penaltyFactor;
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
            if (_gameplayLootService.CollectedLootItems.Count == 0)
                return false;

            foreach (var typeId in _gameplayLootService.CollectedLootItems)
                if (_orderIngredients.good.Any(goodIngredient => typeId == goodIngredient.TypeId))
                    return true;

            return false;
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
            List<LootSetup> availableLoot = new List<LootSetup>(_gameplayLootService.AvailableLoot);

            availableLoot.ShuffleList();
            availableLoot.RemoveAll(setup => setup.CanBeUsedInOrders == false);

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
                orderSetup.MinMaxNeedAmount,
                IngredientTypeId.Good
            );

            FillIngredientsRandom
            (
                availableLoot,
                badCount,
                _orderIngredients.bad,
                minMaxFactor,
                Vector2Int.zero,
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

        private float ApplyPenaltyFactor(List<IngredientData> bad, int total)
        {
            int collectedBad = 0;
            foreach (IngredientData ingredientData in bad)
            {
                int collectedTypes = _gameplayLootService.CollectedLootItems.Count(type => type == ingredientData.TypeId);
                collectedBad += collectedTypes;
            }

            float badCollected = collectedBad * _ordersData.BadIngredientPenaltyFactor;
            float penaltyFactor = 1 - badCollected / total;
            return penaltyFactor;
        }

        private static bool CheckMinDayToUnlock(OrderData data, int currentDay)
        {
            return data.Setup.MinMaxDayToUnlock.x > 0 && currentDay < data.Setup.MinMaxDayToUnlock.x;
        }

        private static bool CheckMaxDayToUnlock(OrderData data, int currentDay)
        {
            return data.Setup.MinMaxDayToUnlock.y > 0 && currentDay > data.Setup.MinMaxDayToUnlock.y;
        }

        private bool CheckTag(OrderData data, int currentDay)
        {
            if (data.Setup.Tag == OrderTag.None)
                return true;

            DayData dayData = _daysService.GetDayData(currentDay);
            
            if (dayData.AvailableOrderTags.Contains(data.Setup.Tag))
                return true;

            return false;
        }
    }
}