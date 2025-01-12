using System;
using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Meta.UI.HardCurrencyHolder.Service;
using Code.Meta.UI.Shop.Configs;

namespace Code.Meta.UI.Shop.Service
{
    public class ShopUIService : IShopUIService
    {
        private readonly List<int> _purchasedItems = new();
        private readonly Dictionary<int, ShopItemData> _availableItems = new();

        public event Action ShopChanged;

        public void UpdatePurchasedItems(IEnumerable<int> purchasedItems)
        {
            _purchasedItems.RefreshList(purchasedItems);
        }

        public void UpdatePurchasedItem(int shopItemId)
        {
            _availableItems.Remove(shopItemId);
            _purchasedItems.Add(shopItemId);

            ShopChanged?.Invoke();
        }

        public List<ShopItemData> GetAvailableShopItems()
        {
            return new List<ShopItemData>(_availableItems.Values);
        }

        public ShopItemData GetConfig(int shopItemId)
        {
            return _availableItems.GetValueOrDefault(shopItemId);
        }

        public bool IsItemPurchased(int shopItemId)
        {
            return _purchasedItems.Contains(shopItemId);
        }
 
        public void CreateBuyRequest(int shopItemId, bool forAd = false)
        {
            CreateMetaEntity.Empty()
                .With(x => x.isBuyRequest = true)
                .With(x => x.AddShopItemId(shopItemId), when: shopItemId is not 0)
                .With(x => x.isForAd = forAd)
                ;
        }

        public void NotifyPurchase()
        {
            ShopChanged?.Invoke();
        }

        public void Cleanup()
        {
            _purchasedItems.Clear();
            _availableItems.Clear();

            ShopChanged = null;
        }
    }
}