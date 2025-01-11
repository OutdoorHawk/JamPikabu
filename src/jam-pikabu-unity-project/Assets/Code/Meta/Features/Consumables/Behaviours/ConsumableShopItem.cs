using Code.Gameplay.Common.Time.Service;
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

        public TMP_Text DurationText;

        private IShopUIService _shopUIService;
        private ILocalizedTimeService _localizedTimeService;
        private IConsumablesUIService _consumablesUIService;

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
            ShopItemView.Initialize(shopItemData);

            DurationText.text = _localizedTimeService.GetLocalizedTime(shopItemData.MinutesDuration * 60);
        }

        private void Refresh()
        {
        }
    }
}