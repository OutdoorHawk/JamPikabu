using System;
using System.Collections.Generic;
using Code.Meta.Features.Consumables.Data;
using Code.Meta.UI.Shop.Configs;

namespace Code.Meta.Features.Consumables.Service
{
    public interface IConsumablesUIService
    {
        event Action OnConsumablesUpdated;
        void InitPurchasedConsumables(IEnumerable<PurchasedConsumableData> purchasedConsumables);
        void PurchaseConsumableExtraLoot(ShopItemData data);
        void ConsumableSpend(ConsumableTypeId type);
        IReadOnlyList<PurchasedConsumableData> GetActiveConsumables();
        void RemoveConsumable(ConsumableTypeId value);
        bool IsActive(ConsumableTypeId lootType);
        int GetActiveTimeLeft(ConsumableTypeId lootType);
        int GetConsumableAmount(ConsumableTypeId lootType);
    }
}