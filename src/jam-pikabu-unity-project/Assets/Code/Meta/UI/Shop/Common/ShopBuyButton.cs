using Code.Common.Extensions;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Currency.Service;
using Code.Meta.UI.Common;
using Code.Meta.UI.Shop.Service;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.UI.Shop.Common
{
    public class ShopBuyButton : MonoBehaviour
    {
        public Button BuyButton;
        public UIShiny Shiny;
        public PriceInfo PurchasePrice;

        public Image AvailableButton;
        public Image NotAvailableButton;
        public Color AvailableColor;
        public Color NotAvailableColor;

        private IGameplayCurrencyService _currency;
        private IShopUIService _shopUIService;

        private CostSetup _currentPrice;
        private bool _firstInitComplete;

        public Button Button => BuyButton;

        [Inject]
        private void Construct
        (
            IGameplayCurrencyService currency,
            IShopUIService shopUIService
        )
        {
            _shopUIService = shopUIService;
            _currency = currency;
        }

        private void OnEnable()
        {
            _shopUIService.ShopChanged += Refresh;
            _currency.CurrencyChanged += Refresh;
        }

        private void OnDisable()
        {
            _shopUIService.ShopChanged -= Refresh;
            _currency.CurrencyChanged -= Refresh;
        }

        public void InitUpgradePrice(CostSetup cost)
        {
            PurchasePrice.SetupPrice(cost, withAnimation: _firstInitComplete);
            _currentPrice = cost;

            Refresh();
        }

        public void Refresh()
        {
            if (_currentPrice == null)
                return;

            ResetAll();

            int amount = _currency.GetCurrencyOfType(_currentPrice.CurrencyType, false);
            bool canBuy = amount >= _currentPrice.Amount;

            if (canBuy)
                SetAvailablePurchase();
            else
                SetNoMoney();
        }

        private void ResetAll()
        {
            Button.enabled = false;
        }

        private void SetNoMoney()
        {
            PurchasePrice.EnableElement();
            SetNotAvailableButtonColors();
        }

        private void SetAvailablePurchase()
        {
            PurchasePrice.EnableElement();
            Button.enabled = true;
            Shiny.Play();
            SetAvailableButtonColors();
        }

        private void SetAvailableButtonColors()
        {
            AvailableButton.EnableElement();
            NotAvailableButton.DisableElement();
            PurchasePrice.AmountText.color = AvailableColor;
        }

        private void SetNotAvailableButtonColors()
        {
            AvailableButton.DisableElement();
            NotAvailableButton.EnableElement();
            PurchasePrice.AmountText.color = NotAvailableColor;
        }
    }
}