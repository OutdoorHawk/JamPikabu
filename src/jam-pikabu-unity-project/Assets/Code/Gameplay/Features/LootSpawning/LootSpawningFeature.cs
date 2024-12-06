using Code.Gameplay.Features.Loot.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.LootSpawning
{
    public sealed class LootSpawningFeature : Feature
    {
        public LootSpawningFeature(ISystemFactory systems)
        {
            Add(systems.Create<InitLootSpawnSystem>());
            Add(systems.Create<SpawnLootSystem>());
            Add(systems.Create<SetLootInitialSpeedSystem>());
            
            Add(systems.Create<EnterNextStateOnLootSpawningComplete>());
        }
    }
}