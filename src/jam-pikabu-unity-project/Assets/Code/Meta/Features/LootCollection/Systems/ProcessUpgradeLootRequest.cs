using Code.Infrastructure.Analytics;
using Code.Meta.Features.LootCollection.Configs;
using Code.Meta.Features.LootCollection.Data;
using Code.Meta.Features.LootCollection.Service;
using Entitas;
using static Code.Infrastructure.Analytics.AnalyticsEventTypes;

namespace Code.Meta.Features.LootCollection.Systems
{
    public class ProcessUpgradeLootRequest : IExecuteSystem
    {
        private readonly ILootCollectionService _lootCollectionService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IGroup<MetaEntity> _lootCollection;
        private readonly IGroup<MetaEntity> _requests;
        private readonly IGroup<MetaEntity> _storages;

        public ProcessUpgradeLootRequest
        (
            MetaContext context,
            ILootCollectionService lootCollectionService,
            IAnalyticsService analyticsService
        )
        {
            _lootCollectionService = lootCollectionService;
            _analyticsService = analyticsService;

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
                    _analyticsService.SendEvent(UpgradeLootTypeFree, request.LootTypeId.ToString());
                    UpgradeLevel(loot);
                    break;
                }

                _analyticsService.SendEvent(UpgradeLootType, loot.LootTypeId.ToString());
                ProcessPaid(request, loot);
            }
        }

        private void UpgradeLevel(MetaEntity loot)
        {
            LootProgressionData lootProgressionData = _lootCollectionService.LootData.GetConfig(loot.LootTypeId);

            if (lootProgressionData != null && loot.Level > lootProgressionData.Levels.Count)
                return;
            
            int newLevel = loot.Level + 1;
            loot.ReplaceLevel(newLevel);
            _lootCollectionService.LootUpgraded(loot.LootTypeId, newLevel: newLevel);
            _analyticsService.SendEvent(LootUpgraded, $"{loot.LootTypeId.ToString()}_LEVEL_{loot.Level}");
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