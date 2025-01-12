using System.Collections.Generic;
using System.Linq;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.Loot;
using Code.Infrastructure.Analytics;
using Code.Meta.Features.Consumables.Data;
using Code.Meta.UI.Shop.Configs;

namespace Code.Meta.Features.Consumables.Service
{
    public class ConsumablesUIService : IConsumablesUIService
    {
        private readonly ITimeService _timeService;
        private readonly IAnalyticsService _analyticsService;

        private readonly Dictionary<LootTypeId, PurchasedConsumableData> _purchasedItems = new();

        public ConsumablesUIService(ITimeService timeService, IAnalyticsService analyticsService)
        {
            _timeService = timeService;
            _analyticsService = analyticsService;
        }

        public void InitPurchasedConsumables(IEnumerable<PurchasedConsumableData> purchasedConsumables)
        {
            _purchasedItems.Clear();
            
            foreach (var purchasedConsumable in purchasedConsumables) 
                _purchasedItems[purchasedConsumable.Type] = purchasedConsumable;
        }

        public void PurchaseConsumableExtraLoot(ShopItemData data)
        {
            if (_purchasedItems.ContainsKey(data.LootType))
                return;

            var itemData = new PurchasedConsumableData
            {
                Type = data.LootType
            };

            if (data.MinutesDuration > 0)
                itemData.ExpirationTime = _timeService.TimeStamp + data.MinutesDuration * 60;

            CreateMetaEntity
                .Empty()
                .With(x => x.isActiveExtraLoot = true)
                .AddLootTypeId(itemData.Type)
                .AddExpirationTime(itemData.ExpirationTime)
                ;

            _purchasedItems.Add(itemData.Type, itemData);
            _analyticsService.SendEvent(AnalyticsEventTypes.Purchase, itemData.Type.ToString());
        }

        public IReadOnlyList<PurchasedConsumableData> GetActiveConsumables()
        {
            return _purchasedItems.Values.ToList();
        }

        public void RemoveActiveExtraLoot(LootTypeId value)
        {
            _purchasedItems.Remove(value);
        }

        public bool IsActive(LootTypeId lootType)
        {
            return _purchasedItems.ContainsKey(lootType) && GetActiveTimeLeft(lootType) > 0;
        }

        public int GetActiveTimeLeft(LootTypeId lootType)
        {
            if (_purchasedItems.TryGetValue(lootType, out PurchasedConsumableData item) == false)
                return 0;
            
            return item.ExpirationTime - _timeService.TimeStamp;
        }
    }
}