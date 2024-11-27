using System;
using System.Collections.Generic;
using Code.Meta.UI.Shop.Configs;

namespace Code.Meta.UI.Shop.Service
{
    public interface IShopUIService
    {
        event Action ShopChanged;
        List<ShopItemConfig> GetAvailableShopItems();
        ShopItemConfig GetConfig(int shopItemId);
        void UpdatePurchasedItems(IEnumerable<int> purchasedItems);
        void UpdatePurchasedItem(int shopItemId);
        bool IsItemPurchased(int shopItemId);
        void CreateBuyRequest(int shopItemId, bool forAd = false);
        void Cleanup();
    }
}