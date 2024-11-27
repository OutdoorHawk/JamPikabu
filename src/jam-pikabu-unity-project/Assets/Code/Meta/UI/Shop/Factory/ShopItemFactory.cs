using Code.Gameplay.StaticData;

namespace Code.Meta.UI.Shop.Factory
{
    public class ShopItemFactory : IShopItemFactory
    {
        private readonly IStaticDataService _staticData;

        public ShopItemFactory
        (
            IStaticDataService staticData
        )
        {
            _staticData = staticData;
        }

        public MetaEntity CreateShopItem(int shopItemId, bool forAd)
        {
            /*ShopItemConfig staticData = _staticData.GetShopItemConfig(shopItemId);
            ShopItemSetup setup = staticData.Data;

            switch (setup.Kind)
            {
                case ShopItemKind.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
*/
            return null;
        }
    }
}