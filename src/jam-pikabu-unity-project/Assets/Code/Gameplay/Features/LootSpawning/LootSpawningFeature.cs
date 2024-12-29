using Code.Gameplay.Features.LootSpawning.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.LootSpawning
{
    public sealed class LootSpawningFeature : Feature
    {
        public LootSpawningFeature(ISystemFactory systems)
        {
            Add(systems.Create<SingleSpawnLootSystem>());
            Add(systems.Create<ContinuousSpawnLootSystem>());
            Add(systems.Create<ConveyorSpawnerLootSystem>());
            Add(systems.Create<SetLootInitialSpeedSystem>());
        }
    }
}