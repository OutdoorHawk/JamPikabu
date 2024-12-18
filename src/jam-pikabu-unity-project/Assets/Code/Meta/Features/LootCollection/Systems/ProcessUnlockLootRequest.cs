using System.Linq;
using Code.Meta.Features.LootCollection.Factory;
using Code.Meta.Features.LootCollection.Service;
using Code.Progress.SaveLoadService;
using Entitas;

namespace Code.Meta.Features.LootCollection.Systems
{
    public class ProcessUnlockLootRequest : IExecuteSystem
    {
        private readonly ILootCollectionFactory _lootCollectionFactory;
        private readonly ILootCollectionService _lootCollection;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IGroup<MetaEntity> _request;
        private readonly IGroup<MetaEntity> _existingLoot;

        public ProcessUnlockLootRequest
        (
            MetaContext context,
            ILootCollectionFactory lootCollectionFactory,
            ILootCollectionService lootCollection,
            ISaveLoadService saveLoadService
        )
        {
            _lootCollectionFactory = lootCollectionFactory;
            _lootCollection = lootCollection;
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

                MetaEntity newLoot = _lootCollectionFactory.UnlockNewLoot(request.LootTypeId);
                _lootCollection.AddNewUnlockedLoot(newLoot.LootTypeId);
                
                if (newLoot.hasFreeUpgradeTimeSeconds) 
                    _lootCollection.FreeUpgradeTimerUpdated(newLoot.LootTypeId, newLoot.NextFreeUpgradeTime);
                
                _saveLoadService.SaveProgress();
            }
        }

        private bool CheckLootAlreadyAcquired(MetaEntity request)
        {
            return _existingLoot.GetEntities().Any(x => x.LootTypeId == request.LootTypeId);
        }
    }
}