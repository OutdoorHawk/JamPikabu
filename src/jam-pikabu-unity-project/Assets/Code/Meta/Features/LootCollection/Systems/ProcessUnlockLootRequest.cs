using System.Linq;
using Code.Meta.Features.LootCollection.Factory;
using Entitas;

namespace Code.Meta.Features.LootCollection.Systems
{
    public class ProcessLootUnlockRequest : IExecuteSystem
    {
        private readonly ILootCollectionFactory _lootCollectionFactory;
        private readonly IGroup<MetaEntity> _request;
        private readonly IGroup<MetaEntity> _existingLoot;

        public ProcessLootUnlockRequest(MetaContext context, ILootCollectionFactory lootCollectionFactory)
        {
            _lootCollectionFactory = lootCollectionFactory;
            
            //TODO: REQUEST COMPONENT
            _request = context.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.LootTypeId
                ));
            
            _existingLoot = context.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.LootTypeId,
                    MetaMatcher.Loot,
                    MetaMatcher.Level
                ));
        }

        public void Execute()
        {
            foreach (var request in _request)
            {
                request.isDestructed = true;

                if (CheckLootAlreadyAcquired(request))
                    continue;

                _lootCollectionFactory.UnlockNewLoot(request.LootTypeId);
            }
        }

        private bool CheckLootAlreadyAcquired(MetaEntity request)
        {
            return _existingLoot.GetEntities().Any(x => x.LootTypeId == request.LootTypeId);
        }
    }
}