using Code.Common.Extensions;
using Code.Gameplay.Common.Time.Behaviours;
using Code.Gameplay.Common.Time.Service;
using Code.Infrastructure.Localization;
using Code.Meta.Features.Consumables.Service;
using Code.Meta.UI.Shop.Common;
using Code.Meta.UI.Shop.Configs;
using Code.Meta.UI.Shop.Service;
using TMPro;
using UnityEngine;
using Zenject;

namespace Code.Meta.Features.Consumables.Behaviours
{
    public class ConsumableDurationShopItem : MonoBehaviour
    {
        public ShopItemView ShopItemView;
        public ShopBuyButton ShopBuyButton;

        public GameObject ActiveGO;
        public UniversalTimer ActiveDurationTimer;

        public GameObject DurationGO;
        public TMP_Text DurationText;
        public TMP_Text AmountText;

        private IShopUIService _shopUIService;
        private ILocalizedTimeService _localizedTimeService;
        private IConsumablesUIService _consumablesUIService;
        private ILocalizationService _localizationService;
        private ShopItemData _shopItemData;

        public ConsumableTypeId ConsumableType { get; private set; }

        [Inject]
        private void Construct
        (
            IShopUIService shopUIService,
            ILocalizedTimeService localizedTimeService,
            IConsumablesUIService consumablesUIService,
            ILocalizationService localizationService
        )
        {
            _localizationService = localizationService;
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

        private void Refresh()
        {
            InitDuration();
            InitBuyButton();
            InitAmount();
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

        private void InitAmount()
        {
            if (_shopItemData.MinutesDuration > 0)
            {
                AmountText.DisableElement();
            }
            else
            {
                AmountText.EnableElement();
                string text = $"{_localizationService["SHOP/CONSUMABLES_AMOUNT"]} {_consumablesUIService.GetConsumableAmount(ConsumableType).ToString()}";
                AmountText.text = text;
            }
        }
    }
}