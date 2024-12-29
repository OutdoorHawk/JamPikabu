using System.Threading;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.StaticData;
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
        [SerializeField] private Image _orderIconPenaltyFilled;
        [SerializeField] private PriceInfo _orderReward;
        [SerializeField] private float _fillDelay = 0.75f;

        private IOrdersService _ordersService;
        private IGameplayLootService _gameplayLootService;

        private IStaticDataService _staticDataService;
        private CancellationTokenSource _fillDelaySource;
        private IGameplayLootService _lootService;
        private Tweener _tweener;

        public PriceInfo Reward => _orderReward;

        [Inject]
        private void Construct(IOrdersService ordersService, IGameplayLootService gameplayLootService, 
            IGameplayLootService lootService, IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
            _lootService = lootService;
            _gameplayLootService = gameplayLootService;
            _ordersService = ordersService;
        }

        private void Start()
        {
            //_ordersService.OnOrderUpdated += InitOrder;
             _lootService.OnLootUpdate += InitOrderFillProgress;
        }

        private void OnDestroy()
        {
            //_ordersService.OnOrderUpdated -= InitOrder;
             _lootService.OnLootUpdate -= InitOrderFillProgress;
        }

        public void InitOrder()
        {
            var currentOrder = _ordersService.GetCurrentOrder();

            _orderIcon.sprite = currentOrder.Setup.OrderIcon;
            _orderIconFilled.sprite = currentOrder.Setup.OrderIcon;
            _orderIconPenaltyFilled.sprite = currentOrder.Setup.OrderIcon;
            _orderIconFilled.fillAmount = 0;
            _orderIconPenaltyFilled.fillAmount = 0;

            Reward.SetupPrice(_ordersService.GetRewardForOrder());
        }

        public void InitOrderFillProgress()
        {
            _fillDelaySource?.Cancel();
            _fillDelaySource = CreateLinkedTokenSource(destroyCancellationToken);
            float delay = _staticDataService.Get<LootSettingsStaticData>().CollectFlyAnimationDuration;

            _tweener?.Kill();
            _tweener = _orderIconFilled
                .DOFillAmount(_ordersService.GetOrderProgress(), 0.25f)
                .SetDelay(delay)
                .SetLink(gameObject)
                .OnComplete(() =>  _orderIconPenaltyFilled
                    .DOFillAmount(1-_ordersService.GetPenaltyFactor(), 0.5f)
                    .SetLink(gameObject))
                ;
        }
    }
}