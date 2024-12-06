using System.Collections.Generic;
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

        private OrdersStaticData _ordersData;
        private int _currentOrderIndex;

        private readonly List<OrderData> _ordersBuffer = new();

        public OrdersService(IStaticDataService staticDataService, IRoundStateService roundStateService, IOrdersFactory ordersFactory)
        {
            _staticDataService = staticDataService;
            _roundStateService = roundStateService;
            _ordersFactory = ordersFactory;
        }

        public void InitDay()
        {
            _ordersData = _staticDataService.GetStaticData<OrdersStaticData>();
            _currentOrderIndex = 0;
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
           return _ordersFactory.CreateOrder(order);
        }

        public OrderData GetCurrentOrder()
        {
            return _ordersBuffer[_currentOrderIndex];
        }

        private static bool CheckMinDayToUnlock(OrderData data, int currentDay)
        {
            return data.Setup.MinDayToUnlock > 0 && currentDay < data.Setup.MinDayToUnlock;
        }

        private static bool CheckMaxDayToUnlock(OrderData data, int currentDay)
        {
            return data.Setup.MaxDayToUnlock > 0 && currentDay > data.Setup.MaxDayToUnlock;
        }

        public void GoToNextOrder()
        {
            if (_currentOrderIndex >= _ordersBuffer.Count)
                _currentOrderIndex = 0;

            _currentOrderIndex++;
        }

        public void GameOver()
        {
            _currentOrderIndex = 0;
            _ordersBuffer.Clear();
        }
    }
}