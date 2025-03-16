using Code.Infrastructure.Systems;
using Code.Meta.Features.Consumables;
using Code.Meta.Features.Days.Systems;
using Code.Meta.Features.ExpirationTimer;
using Code.Meta.Features.LootCollection.Systems;
using Code.Meta.Features.MapBlocks;
using Code.Meta.Features.Storage;

namespace Code.Meta.Features
{
    public sealed class ActualizeProgressFeature : Feature
    {
        public ActualizeProgressFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitializeDaysProgressSystem>());
            Add(systems.Create<InitLootProgressionSystem>());
            Add(systems.Create<InitializeLootFreeUpgradeTimers>());
            
            Add(systems.Create<ExpirationTimerSystem>(1f));

            Add(systems.Create<ExtraLootConsumablesFeature>());
            
            Add(systems.Create<StorageFeature>());
        }
    }
}