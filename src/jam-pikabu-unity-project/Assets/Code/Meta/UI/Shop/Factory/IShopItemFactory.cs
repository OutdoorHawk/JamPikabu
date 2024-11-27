namespace Code.Meta.UI.Shop.Factory
{
    public interface IShopItemFactory
    {
        MetaEntity CreateShopItem(int shopItemId, bool forAd);
    }
}