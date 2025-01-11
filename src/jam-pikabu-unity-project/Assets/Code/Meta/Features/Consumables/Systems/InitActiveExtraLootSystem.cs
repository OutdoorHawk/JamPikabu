using System.Collections.Generic;
using System.Linq;
using Code.Meta.Features.Consumables.Data;
using Code.Meta.Features.Consumables.Service;
using Entitas;

namespace Code.Meta.Features.Consumables.Systems
{
    public class InitActiveExtraLootSystem : IInitializeSystem
    {
        private readonly IGroup<MetaEntity> _purchasedExtraLoot;
        private readonly IConsumablesUIService _consumablesUIService;

        public InitActiveExtraLootSystem(MetaContext context, IConsumablesUIService consumablesUIService)
        {
            _consumablesUIService = consumablesUIService;

            _purchasedExtraLoot = context.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.ActiveExtraLoot,
                    MetaMatcher.LootTypeId
                    )
                .NoneOf(
                    MetaMatcher.Expired));
        }

        public void Initialize()
        {
            IEnumerable<PurchasedConsumableData> purchasedConsumables = _purchasedExtraLoot
                .GetEntities()
                .ToList()
                .Select(Selector);

            _consumablesUIService.InitPurchasedConsumables(purchasedConsumables);
        }

        private static PurchasedConsumableData Selector(MetaEntity x)
        {
            var data = new PurchasedConsumableData
            {
                Type = x.LootTypeId 
            };
            
            if (x.hasExpirationTime)
                data.ExpirationTime = x.ExpirationTime;
            
            return data;
        }
    }
}