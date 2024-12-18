using System.Collections.Generic;
using Code.Meta.Features.LootCollection.Service;
using Entitas;

namespace Code.Meta.Features.LootCollection.Systems
{
    public class InitLootProgressionSystem : IInitializeSystem
    {
        private readonly ILootCollectionService _lootCollectionService;
        private readonly IGroup<MetaEntity> _lootCollection;

        public InitLootProgressionSystem(MetaContext context, ILootCollectionService lootCollectionService)
        {
            _lootCollectionService = lootCollectionService;
            _lootCollection = context.GetGroup(MetaMatcher.AllOf(
                MetaMatcher.Loot,
                MetaMatcher.LootTypeId));
        }

        public void Initialize()
        {
            List<LootItemCollectionData> items = new(_lootCollection.count);
            
            foreach (MetaEntity loot in _lootCollection)
            {
                var item = new LootItemCollectionData(loot.LootTypeId, loot.Level);
                if (loot.hasNextFreeUpgradeTime) 
                    item.NextFreeUpgradeTime = loot.NextFreeUpgradeTime;
                items.Add(item);
            }

            _lootCollectionService.InitializeLootProgression(items);
        }
    }
}