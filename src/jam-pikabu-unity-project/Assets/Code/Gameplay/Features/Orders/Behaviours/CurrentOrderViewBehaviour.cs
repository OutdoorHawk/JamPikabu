using Code.Gameplay.Features.Orders.Service;
using TMPro;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Orders.Behaviours
{
    public class CurrentOrderViewBehaviour : MonoBehaviour
    {
        [SerializeField] private TMP_Text _orderText;

        private IOrdersService _ordersService;

        [Inject]
        private void Construct(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        private void Start()
        {
            _ordersService.OnOrderUpdated += UpdateText;
        }

        private void OnDestroy()
        {
            _ordersService.OnOrderUpdated -= UpdateText;
        }

        private void UpdateText()
        {
            _orderText.text = $"{_ordersService.OrdersCompleted+1}/{_ordersService.MaxOrders}";
        }
    }
}