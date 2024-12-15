using Code.Gameplay.Features.Orders.Service;
using Code.Meta.Features.Days.Service;
using TMPro;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Orders.Behaviours
{
    public class CurrentOrderCountViewBehaviour : MonoBehaviour
    {
        [SerializeField] private TMP_Text _orderText;

        private IOrdersService _ordersService;
        private IDaysService _daysService;

        [Inject]
        private void Construct(IOrdersService ordersService, IDaysService daysService)
        {
            _daysService = daysService;
            _ordersService = ordersService;
        }

        private void Start()
        {
            _daysService.OnDayBegin += InitText;
            _ordersService.OnOrderUpdated += UpdateText;
        }

        private void OnDestroy()
        {
            _daysService.OnDayBegin -= InitText;
            _ordersService.OnOrderUpdated -= UpdateText;
        }

        private void InitText()
        {
            _orderText.text = $"{0}/{_ordersService.MaxOrders}";
        }

        private void UpdateText()
        {
            _orderText.text = $"{_ordersService.OrdersCompleted+1}/{_ordersService.MaxOrders}";
        }
    }
}