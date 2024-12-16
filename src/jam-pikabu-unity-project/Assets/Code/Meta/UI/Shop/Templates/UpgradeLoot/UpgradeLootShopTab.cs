using Code.Common.Extensions;
using Code.Meta.UI.Shop.Configs;
using Code.Meta.UI.Shop.WindowService;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.UI.Shop.Templates.UpgradeLoot
{
    public class UpgradeLootShopTab : BaseShopTab
    {
        public GridLayoutGroup Layout;

        private IShopWindowService _shopWindowService;

        [Inject]
        private void Construct(IShopWindowService shopWindowService)
        {
            _shopWindowService = shopWindowService;
        }

        public override void ActivateTab()
        {
            base.ActivateTab();
            
            gameObject.EnableElement();
            CreateItems();
        }

        private void CreateItems()
        {
            ShopItemTemplateData prefab = _shopWindowService.GetTemplate(ShopItemKind.UpgradeIngredient);
            
        }
    }
}