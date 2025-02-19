using Code.Common.Extensions;
using Code.Gameplay.Common.Time.Behaviours;
using Code.Gameplay.Common.Time.Service;
using Code.Gameplay.Features.Loot;
using Code.Meta.Features.Consumables.Service;
using Code.Meta.UI.Shop.Common;
using Code.Meta.UI.Shop.Configs;
using Code.Meta.UI.Shop.Service;
using TMPro;
using UnityEngine;
using Zenject;

namespace Code.Meta.Features.Consumables.Behaviours
{
    public class ConsumableShopItem : MonoBehaviour
    {
        public ShopItemView ShopItemView;
        public ShopBuyButton ShopBuyButton;

        public GameObject ActiveGO;
        public UniversalTimer ActiveDurationTimer;

        public GameObject DurationGO;
        public TMP_Text DurationText;

        private IShopUIService _shopUIService;
        private ILocalizedTimeService _localizedTimeService;
        private IConsumablesUIService _consumablesUIService;
        private ShopItemData _shopItemData;

        public ConsumableTypeId ConsumableType { get; private set; }

        [Inject]
        private void Construct
        (
            IShopUIService shopUIService,
            ILocalizedTimeService localizedTimeService,
            IConsumablesUIService consumablesUIService
        )
        {
            _consumablesUIService = consumablesUIService;
            _localizedTimeService = localizedTimeService;
            _shopUIService = shopUIService;
        }

        private void OnEnable()
        {
            _shopUIService.ShopChanged += Refresh;
        }

        private void OnDisable()
        {
            _shopUIService.ShopChanged -= Refresh;
        }

        public void Initialize(ShopItemData shopItemData)
        {
            _shopItemData = shopItemData;
            ConsumableType = shopItemData.ConsumableType;
            
            ShopItemView.Initialize(shopItemData);
            ShopBuyButton.InitUpgradePrice(shopItemData.Cost);

            Refresh();
        }

        private void InitDuration()
        {
            if (_consumablesUIService.IsActive(ConsumableType))
            {
                DurationGO.DisableElement();
            }
            else
            {
                DurationGO.EnableElement();
                DurationText.text = _localizedTimeService.GetLocalizedTime(_shopItemData.MinutesDuration * 60);
            }
        }

        private void InitBuyButton()
        {
            if (_consumablesUIService.IsActive(ConsumableType))
            {
               ShopBuyButton.DisableElement();
               ActiveGO.EnableElement();
               ActiveDurationTimer.StartTimer(() => _consumablesUIService.GetActiveTimeLeft(ConsumableType) + 1, Refresh);
            }
            else
            {
                ActiveGO.DisableElement();
                ShopBuyButton.EnableElement();
                ShopBuyButton.Refresh();
            }
        }

        private void Refresh()
        {
            InitDuration();
            InitBuyButton();
        }
    }
}