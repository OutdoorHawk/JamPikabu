using Code.Gameplay.Features.Loot.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.Loot
{
    public sealed class LootFeature : Feature
    {
        public LootFeature(ISystemFactory systems)
        {
            Add(systems.Create<SpawnLootSystem>());
            Add(systems.Create<SetLootInitialSpeedSystem>());
            
            Add(systems.Create<ProcessLootPickup>());
            
            Add(systems.Create<ApplyLootValueSystem>());
            
            Add(systems.Create<DestroyAppliedLootSystem>());
        }
    }
}