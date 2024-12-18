using Code.Infrastructure.Systems;
using Code.Meta.Features.LootCollection.Systems;

namespace Code.Meta.Features.LootCollection
{
    public sealed class LootCollectionFeature : Feature
    {
        public LootCollectionFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitLootProgressionSystem>());
            
            Add(systems.Create<ProcessUpgradeLootRequest>());
            Add(systems.Create<ProcessUnlockLootRequest>());
            Add(systems.Create<ProcessFreeUpgradeLootTimerSystem>(1f));
        }
    }
}