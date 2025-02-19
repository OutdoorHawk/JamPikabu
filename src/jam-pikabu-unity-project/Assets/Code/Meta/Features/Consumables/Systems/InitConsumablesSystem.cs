using System.Collections.Generic;
using System.Linq;
using Code.Meta.Features.Consumables.Data;
using Code.Meta.Features.Consumables.Service;
using Entitas;

namespace Code.Meta.Features.Consumables.Systems
{
    public class InitConsumablesSystem : IInitializeSystem
    {
        private readonly IGroup<MetaEntity> _purchasedConsumables;
        private readonly IConsumablesUIService _consumablesUIService;

        public InitConsumablesSystem(MetaContext context, IConsumablesUIService consumablesUIService)
        {
            _consumablesUIService = consumablesUIService;

            _purchasedConsumables = context.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.ConsumableTypeId,
                    MetaMatcher.Amount
                )
                .NoneOf(
                    MetaMatcher.Expired));
        }

        public void Initialize()
        {
            IEnumerable<PurchasedConsumableData> purchasedConsumables = _purchasedConsumables
                .GetEntities()
                .ToList()
                .Select(Selector);

            _consumablesUIService.InitPurchasedConsumables(purchasedConsumables);
        }

        private static PurchasedConsumableData Selector(MetaEntity x)
        {
            return new PurchasedConsumableData
            (
                x.ConsumableTypeId,
                x.Amount,
                x.hasExpirationTime ? x.ExpirationTime : 0
            );
        }
    }
}