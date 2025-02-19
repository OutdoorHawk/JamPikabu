using System.Collections.Generic;
using Code.Meta.Features.Consumables.Data;
using Code.Meta.UI.Shop.Configs;

namespace Code.Meta.Features.Consumables.Service
{
    public interface IConsumablesUIService
    {
        void InitPurchasedConsumables(IEnumerable<PurchasedConsumableData> purchasedConsumables);
        void PurchaseConsumableExtraLoot(ShopItemData data);
        IReadOnlyList<PurchasedConsumableData> GetActiveConsumables();
        void RemoveConsumable(ConsumableTypeId value);
        bool IsActive(ConsumableTypeId lootType);
        int GetActiveTimeLeft(ConsumableTypeId lootType);
    }
}