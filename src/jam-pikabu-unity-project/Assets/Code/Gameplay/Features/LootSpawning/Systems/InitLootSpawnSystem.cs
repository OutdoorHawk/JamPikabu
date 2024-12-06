using Code.Gameplay.Features.Loot.Factory;
using Entitas;

namespace Code.Gameplay.Features.LootSpawning.Systems
{
    public class InitLootSpawnSystem : IInitializeSystem
    {
        private readonly ILootFactory _lootFactory;

        public InitLootSpawnSystem(ILootFactory lootFactory)
        {
            _lootFactory = lootFactory;
        }

        public void Initialize()
        {
            _lootFactory.CreateLootSpawner();
        }
    }
}