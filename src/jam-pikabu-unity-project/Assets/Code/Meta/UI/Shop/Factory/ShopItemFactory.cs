using System;
using Code.Gameplay.StaticData;
using Code.Meta.Features.Consumables.Service;
using Code.Meta.UI.Shop.Configs;

namespace Code.Meta.UI.Shop.Factory
{
    public class ShopItemFactory : IShopItemFactory
    {
        private readonly IStaticDataService _staticData;
        private readonly IConsumablesUIService _consumablesUIService;

        public ShopItemFactory
        (
            IStaticDataService staticData,
            IConsumablesUIService consumablesUIService
        )
        {
            _staticData = staticData;
            _consumablesUIService = consumablesUIService;
        }

        public MetaEntity CreateShopItem(int shopItemId, bool forAd)
        {
            ShopItemData data = _staticData.Get<ShopStaticData>().GetById(shopItemId);

            switch (data.Kind)
            {
                case ShopItemKind.Unknown:
                    break;
                case ShopItemKind.UpgradeIngredient:
                case ShopItemKind.Consumable:
                    _consumablesUIService.PurchaseConsumableExtraLoot(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return null;
        }
    }
}