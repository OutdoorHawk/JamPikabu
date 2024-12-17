using System.Linq;
using Code.Meta.Features.LootCollection.Factory;
using Code.Progress.SaveLoadService;
using Entitas;

namespace Code.Meta.Features.LootCollection.Systems
{
    public class ProcessUnlockLootRequest : IExecuteSystem
    {
        private readonly ILootCollectionFactory _lootCollectionFactory;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IGroup<MetaEntity> _request;
        private readonly IGroup<MetaEntity> _existingLoot;

        public ProcessUnlockLootRequest(MetaContext context, ILootCollectionFactory lootCollectionFactory, ISaveLoadService saveLoadService)
        {
            _lootCollectionFactory = lootCollectionFactory;
            _saveLoadService = saveLoadService;

            _request = context.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.UnlockLootRequest,
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
                _saveLoadService.SaveProgress();
            }
        }

        private bool CheckLootAlreadyAcquired(MetaEntity request)
        {
            return _existingLoot.GetEntities().Any(x => x.LootTypeId == request.LootTypeId);
        }
    }
}