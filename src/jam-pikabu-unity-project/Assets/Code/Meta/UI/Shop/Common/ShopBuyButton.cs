using Code.Common.Extensions;
using Code.Gameplay.Features.Currency.Config;
using Code.Meta.UI.Common;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;

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

        private CostSetup _upgradePrice;
        private bool _firstInitComplete;

        public Button Button => BuyButton;

        public void InitUpgradePrice(CostSetup cost)
        {
            PurchasePrice.SetupPrice(cost, withAnimation: _firstInitComplete);
            _upgradePrice = null;
            _upgradePrice = cost;
        }

        public void ResetAll()
        {
            PurchasePrice.DisableElement();
            Button.enabled = false;
        }

        public void SetMaxLevelReached()
        {
            Shiny.Stop();
            SetAvailableButtonColors();
        }

        public void SetNoMoneyForUpgrade()
        {
            PurchasePrice.EnableElement();
            SetNotAvailableButtonColors();
        }

        public void SetCanUpgrade()
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