using System.Collections.Generic;
using Code.Gameplay.Features.Customers.Config;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.StaticData;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using RoyalGold.Sources.Scripts.Game.MVC.Utils;
using Zenject;

namespace Code.Gameplay.Features.Customers.Service
{
    public class CustomersService : ICustomersService, IConfigsInitHandler
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IDaysService _daysService;
        private readonly IOrdersService _ordersService;

        private readonly List<CustomerSetup> _configs = new();
        private int _currentCustomerId;

        private CustomerStaticData CustomerData => _staticDataService.GetStaticData<CustomerStaticData>();

        [Inject]
        private CustomersService(IStaticDataService staticDataService, IDaysService daysService, IOrdersService ordersService)
        {
            _daysService = daysService;
            _ordersService = ordersService;
            _staticDataService = staticDataService;
        }

        public void OnConfigsInitInitComplete()
        {
            foreach (var setup in CustomerData.Configs)
                _configs.Add(setup);

            _configs.ShuffleList();
            _ordersService.OnOrderUpdated += SetNewCustomer;
        }

        public CustomerSetup GetCustomerSetup()
        {
            DayData dayData = _daysService.GetDayData();
            if (dayData == null)
                return _configs[_currentCustomerId];
            
            if (dayData.IsBossDay)
                return _configs.Find(setup => setup.IsBossCustomer);

            if (_currentCustomerId >= _configs.Count)
                return _configs[^1];
            
            return _configs[_currentCustomerId];
        }

        private void SetNewCustomer()
        {
            if (_currentCustomerId >= _configs.Count)
                _currentCustomerId = 0;

            _currentCustomerId++;
        }
    }
}