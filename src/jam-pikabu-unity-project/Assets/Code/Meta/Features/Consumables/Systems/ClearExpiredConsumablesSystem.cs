using Code.Meta.Features.Consumables.Service;
using Entitas;

namespace Code.Meta.Features.Consumables.Systems
{
    public class ClearExpiredConsumablesSystem : IExecuteSystem
    {
        private readonly IConsumablesUIService _consumablesUIService;
        private readonly IGroup<MetaEntity> _entities;

        public ClearExpiredConsumablesSystem(MetaContext context, IConsumablesUIService consumablesUIService)
        {
            _consumablesUIService = consumablesUIService;

            _entities = context.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.ConsumableTypeId,
                    MetaMatcher.Expired
                ));
        }

        public void Execute()
        {
            foreach (var entity in _entities)
            {
                _consumablesUIService.RemoveConsumable(entity.ConsumableTypeId);
            }
        }
    }
}