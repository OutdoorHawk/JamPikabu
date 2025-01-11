using System.Collections.Generic;
using Code.Gameplay.Features.Cooldowns;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.StaticData;
using Entitas;

namespace Code.Gameplay.Features.LootSpawning.Systems
{
    public class ContinuousSpawnLootSystem : IExecuteSystem
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IGameplayLootService _gameplayLootService;

        private readonly IGroup<GameEntity> _entities;
        private readonly IGroup<GameEntity> _ingredientLoot;
        private readonly IGroup<GameEntity> _extraLoot;

        private readonly List<GameEntity> _buffer = new(2);

        private LootSettingsStaticData LootStaticData => _staticDataService.Get<LootSettingsStaticData>();

        public ContinuousSpawnLootSystem(GameContext context, IStaticDataService staticDataService,
            IGameplayLootService gameplayLootService)
        {
            _staticDataService = staticDataService;
            _gameplayLootService = gameplayLootService;

            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.LootSpawner,
                    GameMatcher.ContinuousSpawn,
                    GameMatcher.LootSpawnInterval,
                    GameMatcher.CooldownUp
                ));

            _ingredientLoot = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Loot,
                    GameMatcher.View,
                    GameMatcher.Rating
                ));

            _extraLoot = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Loot,
                    GameMatcher.View
                ).NoneOf(
                    GameMatcher.Rating));
        }

        public void Execute()
        {
            foreach (var spawner in _entities.GetEntities(_buffer))
            {
                if (_ingredientLoot.count >= LootStaticData.MaxIngredientLootAmount)
                {
                    spawner.isDestructed = true;
                    continue;
                }

                spawner.PutOnCooldown(spawner.LootSpawnInterval);

                _gameplayLootService.TrySpawnIngredientLoot();

                if (_extraLoot.count >= LootStaticData.MaxExtraLootAmount)
                    continue;

                _gameplayLootService.TrySpawnExtraLoot();
            }
        }
    }
}