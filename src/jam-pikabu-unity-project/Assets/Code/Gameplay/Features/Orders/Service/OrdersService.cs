using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Features.Orders.Factory;
using Code.Gameplay.Features.RoundState.Service;
using Code.Gameplay.StaticData;
using RoyalGold.Sources.Scripts.Game.MVC.Utils;
using UnityEngine;

namespace Code.Gameplay.Features.Orders.Service
{
    public class OrdersService : IOrdersService
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IRoundStateService _roundStateService;
        private readonly IOrdersFactory _ordersFactory;
        private readonly ILootService _lootService;

        private OrdersStaticData _ordersData;
        private int _currentOrderIndex;
        private int _ordersCompleted;

        private readonly List<OrderData> _ordersBuffer = new();
        private readonly Dictionary<LootTypeId, IngredientData> _orderIngredientCostDict = new();

        public OrdersService(IStaticDataService staticDataService,
            IRoundStateService roundStateService, IOrdersFactory ordersFactory, ILootService lootService)
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
            var order = GetCurrentOrder();
            InitIngredientsDic(order);
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

        public bool OrdersCompleted()
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
        }

        public void GameOver()
        {
            _currentOrderIndex = 0;
            _ordersBuffer.Clear();
        }

        private void InitIngredientsDic(OrderData order)
        {
            _orderIngredientCostDict.Clear();
            FillIngredients(order.Setup.GoodIngredients);
            FillIngredients(order.Setup.BadIngredients);
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