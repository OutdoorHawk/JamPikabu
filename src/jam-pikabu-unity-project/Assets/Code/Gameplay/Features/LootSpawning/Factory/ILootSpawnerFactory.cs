using System.Collections.Generic;
using Code.Gameplay.Features.Loot;

namespace Code.Gameplay.Features.LootSpawning.Factory
{
    public interface ILootSpawnerFactory
    {
        GameEntity CreateLootSpawner();
        GameEntity CreateOneTimeSpawner(List<LootTypeId> lootToSpawn);
    }
}