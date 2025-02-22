using System.Collections.Generic;
using Code.Gameplay.Features.Cooldowns;
using Code.Gameplay.Features.Loot.Service;
using Entitas;

namespace Code.Gameplay.Features.LootSpawning.Systems
{
    public class OneTimeLootSpawnerSystem : IExecuteSystem
    {
        private readonly IGameplayLootService _gameplayLootService;
        private readonly IGroup<GameEntity> _spawners;

        private readonly List<GameEntity> _buffer = new(1);

        public OneTimeLootSpawnerSystem(GameContext context, IGameplayLootService gameplayLootService)
        {
            _gameplayLootService = gameplayLootService;

            _spawners = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.LootSpawner,
                    GameMatcher.OneTimeSpawner,
                    GameMatcher.LootSpawnInterval,
                    GameMatcher.CooldownUp,
                    GameMatcher.LootToSpawn
                ));
        }

        public void Execute()
        {
            foreach (var spawner in _spawners.GetEntities(_buffer))
            {
                spawner.PutOnCooldown(spawner.LootSpawnInterval);

                var type = spawner.LootToSpawn[0];
                spawner.LootToSpawn.RemoveAt(0);

                _gameplayLootService.SpawnLoot(type);

                if (spawner.LootToSpawn.Count <= 0)
                    spawner.isDestructed = true;
            }
        }
    }
}