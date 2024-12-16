using System.Threading;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Meta.UI.Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static System.Threading.CancellationTokenSource;

namespace Code.Gameplay.Features.Orders.Behaviours
{
    public class OrderViewBehaviour : MonoBehaviour
    {
        [SerializeField] private Image _orderIcon;
        [SerializeField] private Image _orderIconFilled;
        [SerializeField] private PriceInfo _orderReward;
        [SerializeField] private float _fillDelay = 0.75f;

        private IOrdersService _ordersService;
        private IGameplayLootService _gameplayLootService;

        private CancellationTokenSource _fillDelaySource;
        private Tweener _tweener;

        public PriceInfo Reward => _orderReward;

        [Inject]
        private void Construct(IOrdersService ordersService, IGameplayLootService gameplayLootService)
        {
            _gameplayLootService = gameplayLootService;
            _ordersService = ordersService;
        }

        private void Start()
        {
            //_ordersService.OnOrderUpdated += InitOrder;
            // _lootService.OnLootUpdate += InitOrderFillProgress;
        }

        private void OnDestroy()
        {
            //_ordersService.OnOrderUpdated -= InitOrder;
            // _lootService.OnLootUpdate -= InitOrderFillProgress;
        }

        public void InitOrder()
        {
            var currentOrder = _ordersService.GetCurrentOrder();

            _orderIcon.sprite = currentOrder.Setup.OrderIcon;
            _orderIconFilled.sprite = currentOrder.Setup.OrderIcon;
            _orderIconFilled.fillAmount = 0;

            Reward.SetupPrice(currentOrder.Setup.Reward);
        }

        public void InitOrderFillProgress()
        {
            _fillDelaySource?.Cancel();
            _fillDelaySource = CreateLinkedTokenSource(destroyCancellationToken);

            _tweener?.Kill();
            _tweener = _orderIconFilled
                .DOFillAmount(_ordersService.GetOrderProgress(), 0.25f)
                .SetLink(gameObject);
        }
    }
}