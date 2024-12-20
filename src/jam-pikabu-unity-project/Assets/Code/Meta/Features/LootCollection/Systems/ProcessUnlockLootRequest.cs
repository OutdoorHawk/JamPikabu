using System.Linq;
using Code.Meta.Features.LootCollection.Factory;
using Code.Meta.Features.LootCollection.Service;
using Entitas;

namespace Code.Meta.Features.LootCollection.Systems
{
    public class ProcessUnlockLootRequest : IExecuteSystem
    {
        private readonly ILootCollectionFactory _lootCollectionFactory;
        private readonly ILootCollectionService _lootCollection;
        private readonly IGroup<MetaEntity> _request;
        private readonly IGroup<MetaEntity> _existingLoot;

        public ProcessUnlockLootRequest
        (
            MetaContext context,
            ILootCollectionFactory lootCollectionFactory,
            ILootCollectionService lootCollection
        )
        {
            _lootCollectionFactory = lootCollectionFactory;
            _lootCollection = lootCollection;

            _request = context.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.UnlockLootRequest,
                    MetaMatcher.LootTypeId
                ));

            _existingLoot = context.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.LootTypeId,
                    MetaMatcher.LootProgression,
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

                MetaEntity newLoot = _lootCollectionFactory.CreateNewLootProgressionEntity(request.LootTypeId);
                _lootCollection.AddNewUnlockedLoot(newLoot.LootTypeId);
            }
        }

        private bool CheckLootAlreadyAcquired(MetaEntity request)
        {
            return _existingLoot.GetEntities().Any(x => x.LootTypeId == request.LootTypeId);
        }
    }
}