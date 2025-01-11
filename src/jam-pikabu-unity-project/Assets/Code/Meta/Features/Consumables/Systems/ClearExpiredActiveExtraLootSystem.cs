using Code.Meta.Features.Consumables.Service;
using Entitas;

namespace Code.Meta.Features.Consumables.Systems
{
    public class ClearExpiredActiveExtraLootSystem : IExecuteSystem
    {
        private readonly IConsumablesUIService _consumablesUIService;
        private readonly IGroup<MetaEntity> _entities;

        public ClearExpiredActiveExtraLootSystem(MetaContext context, IConsumablesUIService consumablesUIService)
        {
            _consumablesUIService = consumablesUIService;
            _entities = context.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.ActiveExtraLoot,
                    MetaMatcher.Expired
                ));
        }

        public void Execute()
        {
            foreach (var entity in _entities)
            {
                entity.isDestructed = true;
                
                _consumablesUIService.RemoveActiveExtraLoot(entity.lootTypeId.Value);
            }
        }
    }
}