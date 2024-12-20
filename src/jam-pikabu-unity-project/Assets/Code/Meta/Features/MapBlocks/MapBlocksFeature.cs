using Code.Infrastructure.Systems;
using Code.Meta.Features.MapBlocks.Systems;

namespace Code.Meta.Features.MapBlocks
{
    public sealed class MapBlocksFeature : Feature
    {
        public MapBlocksFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitializeLootFreeUpgradeTimers>());
            
            Add(systems.Create<CreateLootFreeUpgradeTimerOnNewLootCreatedSystem>());

            Add(systems.Create<UpdateTimerOnFreeUpgradeLootRequestSystem>());

            Add(systems.Create<ProcessFreeUpgradeLootTimerSystem>(1f));
        }
    }
}