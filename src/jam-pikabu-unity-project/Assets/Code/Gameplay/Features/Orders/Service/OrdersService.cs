using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Features.Orders.Factory;
using Code.Gameplay.Features.RoundState.Service;
using Code.Gameplay.StaticData;
using RoyalGold.Sources.Scripts.Game.MVC.Utils;

namespace Code.Gameplay.Features.Orders.Service
{
    public class OrdersService : IOrdersService
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IRoundStateService _roundStateService;
        private readonly IOrdersFactory _ordersFactory;
        private readonly ILootUIService _lootUIService;

        private OrdersStaticData _ordersData;
        private int _currentOrderIndex;
        private int _ordersCompleted;

        private readonly List<OrderData> _ordersBuffer = new();
        private readonly Dictionary<LootTypeId, IngredientData> _orderIngredientCostDict = new();

        public OrdersService(IStaticDataService staticDataService,
            IRoundStateService roundStateService, IOrdersFactory ordersFactory, ILootUIService lootUIService)
        {
            _staticDataService = staticDataService;
            _roundStateService = roundStateService;
            _ordersFactory = ordersFactory;
            _lootUIService = lootUIService;
        }

        public void InitDay()
        {
            _ordersData = _staticDataService.GetStaticData<OrdersStaticData>();
            _currentOrderIndex = 0;
            _ordersCompleted = 0;
            _ordersBuffer.Clear();

            InitCurrentDayOrders();
        }

        private void InitCurrentDayOrders()
        {
            int currentDay = _roundStateService.CurrentDay;
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

        public OrderData GetCurrentOrder()
        {
            return _ordersBuffer[_currentOrderIndex];
        }

        public void GoToNextOrder()
        {
            _lootUIService.ClearCollectedLoot();
            _ordersCompleted++;

            if (_ordersCompleted >= _ordersData.OrdersAmountInDay)
            {
                _roundStateService.DayComplete();
                return;
            }

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
            foreach (IngredientData ingredientData in order.Setup.GoodIngredients)
                _orderIngredientCostDict.Add(ingredientData.TypeId, ingredientData);
            foreach (IngredientData ingredientData in order.Setup.BadIngredients)
                _orderIngredientCostDict.Add(ingredientData.TypeId, ingredientData);
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