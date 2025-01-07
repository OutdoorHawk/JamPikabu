using Code.Meta.UI.Common;
using Code.Meta.UI.Shop.Configs;
using Code.Meta.UI.Shop.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.UI.Shop.Common
{
    public class ShopItemView : MonoBehaviour
    {
        public TMP_Text Name;
        public TMP_Text Description;
        public Image Icon;
        public PriceInfo Cost;
        public Button BuyButton;
        
        private IShopUIService _shopUIService;
        
        public ShopItemData Data { get; private set; }

        [Inject]
        private void Construct(IShopUIService shopUIService)
        {
            _shopUIService = shopUIService;
        }

        public void Initialize(ShopItemData data)
        {
            Data = data;
            Name.text = data.NameLocale.GetLocalizedString();
            Description.text = data.DescriptionLocale.GetLocalizedString();
            Icon.sprite = data.Icon;
            Cost.SetupPrice(data.Cost);
            BuyButton.onClick.AddListener(PurchaseItem);
        }

        private void PurchaseItem()
        {
            _shopUIService.CreateBuyRequest(Data.Id);
        }
    }
}