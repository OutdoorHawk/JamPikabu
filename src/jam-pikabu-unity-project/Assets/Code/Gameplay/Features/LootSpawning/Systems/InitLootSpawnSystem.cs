using Code.Gameplay.Features.Loot.Factory;
using Code.Gameplay.Features.Loot.Service;
using Entitas;

namespace Code.Gameplay.Features.LootSpawning.Systems
{
    public class InitLootSpawnSystem : IInitializeSystem
    {
        private readonly ILootFactory _lootFactory;
        private readonly ILootService _lootService;

        public InitLootSpawnSystem(ILootFactory lootFactory, ILootService lootService)
        {
            _lootFactory = lootFactory;
            _lootService = lootService;
        }

        public void Initialize()
        {
            _lootFactory.CreateLootSpawner();
            _lootService.InitLootBuffer();
        }
    }
}