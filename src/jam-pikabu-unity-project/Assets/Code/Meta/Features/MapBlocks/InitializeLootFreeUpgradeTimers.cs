using System.Collections.Generic;
using Code.Meta.Features.LootCollection.Data;
using Code.Meta.Features.LootCollection.Service;
using Entitas;

namespace Code.Meta.Features.MapBlocks
{
    public class InitializeLootFreeUpgradeTimers : IInitializeSystem
    {
        private readonly ILootCollectionService _lootCollectionService;
        private readonly IGroup<MetaEntity> _lootCollection;

        public InitializeLootFreeUpgradeTimers(MetaContext context, ILootCollectionService lootCollectionService)
        {
            _lootCollectionService = lootCollectionService;
            _lootCollection = context.GetGroup(MetaMatcher.AllOf(
                MetaMatcher.LootFreeUpgradeTimer,
                MetaMatcher.LootTypeId));
        }

        public void Initialize()
        {
            List<LootFreeUpgradeTimerData> items = new(_lootCollection.count);

            foreach (MetaEntity loot in _lootCollection)
            {
                var item = new LootFreeUpgradeTimerData(loot.LootTypeId, loot.NextFreeUpgradeTime);
                items.Add(item);
            }

            _lootCollectionService.InitializeLootFreeUpgradeTimers(items);
        }
    }
}