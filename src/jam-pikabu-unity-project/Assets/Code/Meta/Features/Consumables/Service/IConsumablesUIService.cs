using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Meta.Features.Consumables.Data;
using Code.Meta.UI.Shop.Configs;

namespace Code.Meta.Features.Consumables.Service
{
    public interface IConsumablesUIService
    {
        void InitPurchasedConsumables(IEnumerable<PurchasedConsumableData> purchasedConsumables);
        void PurchaseConsumableExtraLoot(ShopItemData data);
        IReadOnlyList<PurchasedConsumableData> GetActiveConsumables();
        void RemoveActiveExtraLoot(LootTypeId value);
        bool IsActive(LootTypeId lootType);
        int GetActiveTimeLeft(LootTypeId lootType);
    }
}