using Code.Common.Entity;
using Entitas;

namespace Code.Meta.Features.Consumables.Systems
{
    public class UpdateConsumablesSystem : IExecuteSystem
    {
        private readonly IGroup<MetaEntity> _purchasedConsumables;
        private readonly IGroup<MetaEntity> _updateRequests;

        public UpdateConsumablesSystem(MetaContext context)
        {
            _purchasedConsumables = context.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.ConsumableTypeId,
                    MetaMatcher.Amount
                )
                .NoneOf(
                    MetaMatcher.UpdateConsumableRequest));

            _updateRequests = context.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.UpdateConsumableRequest,
                    MetaMatcher.ConsumableTypeId,
                    MetaMatcher.Amount
                ));
        }

        public void Execute()
        {
            foreach (MetaEntity updateRequest in _updateRequests)
            {
                if (TryFindExistingConsumable(updateRequest, _purchasedConsumables, out MetaEntity consumable))
                {
                    consumable.ReplaceAmount(updateRequest.Amount);
                }
                else
                {
                    consumable = CreateMetaEntity
                        .Empty()
                        .AddConsumableTypeId(updateRequest.ConsumableTypeId)
                        .AddAmount(updateRequest.Amount)
                        ;
                }

                if (updateRequest.hasExpirationTime) 
                    consumable.AddExpirationTime(updateRequest.ExpirationTime);

                updateRequest.isDestructed = true;
            }
        }

        private bool TryFindExistingConsumable(MetaEntity updateRequest, IGroup<MetaEntity> purchasedConsumables, out MetaEntity existing)
        {
            foreach (MetaEntity purchasedConsumable in purchasedConsumables)
            {
                if (purchasedConsumable.ConsumableTypeId != updateRequest.ConsumableTypeId)
                    continue;

                existing = purchasedConsumable;
                return true;
            }

            existing = null;
            return false;
        }
    }
}