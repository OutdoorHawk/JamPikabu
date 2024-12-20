using Code.Infrastructure.Systems;
using Code.Meta.Features.Days.Systems;
using Code.Meta.Features.LootCollection.Systems;
using Code.Meta.Features.MapBlocks;

namespace Code.Meta.Features
{
    public sealed class ActualizeProgressFeature : Feature
    {
        public ActualizeProgressFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitializeDaySystem>());
            Add(systems.Create<InitLootProgressionSystem>());
            Add(systems.Create<InitializeLootFreeUpgradeTimers>());
        }
    }
}