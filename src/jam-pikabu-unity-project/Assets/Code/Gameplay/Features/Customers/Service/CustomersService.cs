using System.Collections.Generic;
using Code.Gameplay.Features.Customers.Config;
using Code.Gameplay.Features.RoundState.Service;
using Code.Gameplay.StaticData;
using RoyalGold.Sources.Scripts.Game.MVC.Utils;
using Zenject;

namespace Code.Gameplay.Features.Customers.Service
{
    public class CustomersService : ICustomersService, IConfigsInitHandler
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IRoundStateService _roundStateService;

        private readonly List<CustomerSetup> _configs = new();
        private int _currentCustomerId;

        private CustomerStaticData CustomerData => _staticDataService.GetStaticData<CustomerStaticData>();

        [Inject]
        private CustomersService(IStaticDataService staticDataService, IRoundStateService roundStateService)
        {
            _roundStateService = roundStateService;
            _staticDataService = staticDataService;
        }

        public void OnConfigsInitInitComplete()
        {
            foreach (var setup in CustomerData.Configs)
                _configs.Add(setup);

            _configs.ShuffleList();
            _roundStateService.OnEnterRoundPreparation += SetNewCustomer;
        }

        public CustomerSetup GetCustomerSetup()
        {
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