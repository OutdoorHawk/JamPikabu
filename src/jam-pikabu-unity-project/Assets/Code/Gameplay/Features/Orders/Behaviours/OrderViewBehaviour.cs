using System;
using Code.Gameplay.Features.Orders.Service;
using Code.Meta.UI.Common;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Features.Orders.Behaviours
{
    public class OrderViewBehaviour : MonoBehaviour
    {
        [SerializeField] private Image _orderIcon;
        [SerializeField] private PriceInfo _orderReward;
        
        private IOrdersService _ordersService;

        [Inject]
        private void Construct(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        private void Start()
        {
            _ordersService.OnOrderUpdated += InitOrder;
        }

        private void OnDestroy()
        {
            _ordersService.OnOrderUpdated -= InitOrder;
        }

        private void InitOrder()
        {
            var currentOrder = _ordersService.GetCurrentOrder();

            _orderIcon.sprite = currentOrder.Setup.OrderIcon;

            _orderReward.SetupPrice(currentOrder.Setup.Reward);
        }
    }
}