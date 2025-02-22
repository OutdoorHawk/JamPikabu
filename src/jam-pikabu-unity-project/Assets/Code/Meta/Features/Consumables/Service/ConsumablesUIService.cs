using System;
using System.Collections.Generic;
using System.Linq;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Common.Time;
using Code.Gameplay.StaticData;
using Code.Infrastructure.Analytics;
using Code.Meta.Features.Consumables.Data;
using Code.Meta.UI.Shop.Configs;
using UnityEngine;

namespace Code.Meta.Features.Consumables.Service
{
    public class ConsumablesUIService : IConsumablesUIService
    {
        private readonly ITimeService _timeService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IStaticDataService _staticDataService;

        private readonly Dictionary<ConsumableTypeId, PurchasedConsumableData> _purchasedItems = new();

        public event Action OnConsumablesUpdated;
        
        private ShopStaticData ShopStaticData => _staticDataService.Get<ShopStaticData>();

        public ConsumablesUIService(ITimeService timeService, 
            IAnalyticsService analyticsService, 
            IStaticDataService staticDataService)
        {
            _timeService = timeService;
            _analyticsService = analyticsService;
            _staticDataService = staticDataService;
        }

        public void InitPurchasedConsumables(IEnumerable<PurchasedConsumableData> purchasedConsumables)
        {
            _purchasedItems.Clear();

            foreach (var purchasedConsumable in purchasedConsumables)
                _purchasedItems[purchasedConsumable.Type] = purchasedConsumable;
        }

        public void AddConsumable(ConsumableTypeId typeId)
        {
            ShopItemData data = ShopStaticData.GetByConsumableType(typeId);
            AddConsumable(data);
        }

        public void AddConsumable(ShopItemData data)
        {
            if (_purchasedItems.TryGetValue(data.ConsumableType, out PurchasedConsumableData purchasedConsumable) == false)
                purchasedConsumable = CreateNewConsumable(data);
            else
                purchasedConsumable = GetStackedConsumable(data, purchasedConsumable);

            CreateMetaEntity
                .Empty()
                .With(x => x.isUpdateConsumableRequest = true)
                .AddConsumableTypeId(purchasedConsumable.Type)
                .AddAmount(purchasedConsumable.Amount)
                .With(x => x.AddExpirationTime(purchasedConsumable.ExpirationTime), purchasedConsumable.ExpirationTime > 0)
                ;

            _purchasedItems[data.ConsumableType] = purchasedConsumable;
            _analyticsService.SendEvent(AnalyticsEventTypes.Purchase, purchasedConsumable.Type.ToString());
            NotifyUpdated();
        }

        public void SpendConsumable(ConsumableTypeId type)
        {
            if (_purchasedItems.TryGetValue(type, out PurchasedConsumableData purchasedConsumable) == false)
                return;

            int newAmount = Mathf.Max(0, purchasedConsumable.Amount - 1);
            _purchasedItems[type] = purchasedConsumable.SetAmount(newAmount);

            CreateUpdateRequest(type);
            NotifyUpdated();
            _analyticsService.SendEvent(AnalyticsEventTypes.SpendConsumable, type.ToString());
        }

        public IReadOnlyList<PurchasedConsumableData> GetActiveConsumables()
        {
            return _purchasedItems.Values.ToList();
        }

        public void RemoveConsumable(ConsumableTypeId value)
        {
            _purchasedItems.Remove(value);
        }

        public bool IsActive(ConsumableTypeId lootType)
        {
            return _purchasedItems.ContainsKey(lootType) && GetActiveTimeLeft(lootType) > 0;
        }

        public int GetActiveTimeLeft(ConsumableTypeId lootType)
        {
            if (_purchasedItems.TryGetValue(lootType, out PurchasedConsumableData item) == false)
                return 0;

            return item.ExpirationTime - _timeService.TimeStamp;
        }

        public bool ConsumableExist(ConsumableTypeId lootType)
        {
            return _purchasedItems.ContainsKey(lootType);
        }

        public int GetConsumableAmount(ConsumableTypeId lootType)
        {
            return _purchasedItems.TryGetValue(lootType, out PurchasedConsumableData item) ? item.Amount : 0;
        }

        private PurchasedConsumableData CreateNewConsumable(ShopItemData data)
        {
            int expirationTime = data.MinutesDuration > 0
                ? _timeService.TimeStamp + data.MinutesDuration * 60
                : 0;

            return new PurchasedConsumableData
            (
                type: data.ConsumableType,
                amount: 1,
                expirationTime
            );
        }

        private static PurchasedConsumableData GetStackedConsumable(ShopItemData data, in PurchasedConsumableData purchasedConsumable)
        {
            return new PurchasedConsumableData
            (
                type: data.ConsumableType,
                amount: purchasedConsumable.Amount + 1
            );
        }

        private void NotifyUpdated()
        {
            OnConsumablesUpdated?.Invoke();
        }

        private void CreateUpdateRequest(ConsumableTypeId type)
        {
            PurchasedConsumableData consumableData = _purchasedItems[type];
            
            CreateMetaEntity
                .Empty()
                .With(x => x.isUpdateConsumableRequest = true)
                .AddConsumableTypeId(consumableData.Type)
                .AddAmount(consumableData.Amount)
                .With(x => x.AddExpirationTime(consumableData.ExpirationTime), consumableData.ExpirationTime > 0)
                ;
        }
    }
}