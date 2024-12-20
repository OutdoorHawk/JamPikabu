using Code.Meta.Features.LootCollection.Service;
using Entitas;

namespace Code.Meta.Features.LootCollection.Systems
{
    public class ProcessUpgradeLootRequest : IExecuteSystem
    {
        private readonly ILootCollectionService _lootCollectionService;
        private readonly IGroup<MetaEntity> _lootCollection;
        private readonly IGroup<MetaEntity> _requests;
        private readonly IGroup<MetaEntity> _storages;

        public ProcessUpgradeLootRequest
        (
            MetaContext context,
            ILootCollectionService lootCollectionService
        )
        {
            _lootCollectionService = lootCollectionService;

            _requests = context.GetGroup(MetaMatcher.AllOf(
                MetaMatcher.UpgradeLootRequest,
                MetaMatcher.LootTypeId));

            _lootCollection = context.GetGroup(MetaMatcher.AllOf(
                MetaMatcher.LootProgression,
                MetaMatcher.LootTypeId));

            _storages = context.GetGroup(MetaMatcher.AllOf(
                MetaMatcher.Gold,
                MetaMatcher.Storage));
        }

        public void Execute()
        {
            foreach (MetaEntity request in _requests)
            foreach (MetaEntity loot in _lootCollection)
            {
                if (request.LootTypeId != loot.LootTypeId)
                    continue;
                
                request.isDestructed = true;

                bool isFreeUpgrade = request.isFreeUpgradeRequest;
                
                if (isFreeUpgrade)
                {
                    UpgradeLevel(loot);
                    break;
                }

                ProcessPaid(request, loot);
            }
        }

        private void UpgradeLevel(MetaEntity loot)
        {
            int newLevel = loot.Level + 1;
            loot.ReplaceLevel(newLevel);
            _lootCollectionService.LootUpgraded(loot.LootTypeId, newLevel: newLevel);
        }

        private void ProcessPaid(MetaEntity request, MetaEntity loot)
        {
            foreach (MetaEntity storage in _storages)
            {
                if (storage.Gold < request.Gold)
                    continue;

                storage.ReplaceGold(storage.Gold - request.Gold);
                
                if (request.hasWithdraw) 
                    storage.ReplaceWithdraw(storage.Withdraw + request.Withdraw);
                
                UpgradeLevel(loot);
            }
        }
    }
}