using System.Collections.Generic;
using Code.Gameplay.Features.Cooldowns;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Factory;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.StaticData;
using Code.Infrastructure.SceneContext;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.LootSpawning.Systems
{
    public class ConveyorSpawnerLootSystem : IExecuteSystem
    {
        private readonly IStaticDataService _staticDataService;
        private readonly ISceneContextProvider _provider;
        private readonly IGameplayLootService _gameplayLootService;
        private readonly ILootFactory _lootFactory;
        private readonly IGroup<GameEntity> _entities;
        private readonly IGroup<GameEntity> _activeLoot;
        private readonly List<GameEntity> _buffer = new(2);

        private int _currentConfig;

        private LootSettingsStaticData LootStaticData => _staticDataService.Get<LootSettingsStaticData>();

        public ConveyorSpawnerLootSystem(GameContext context, IStaticDataService staticDataService,
            ISceneContextProvider provider, IGameplayLootService gameplayLootService, ILootFactory lootFactory)
        {
            _staticDataService = staticDataService;
            _provider = provider;
            _gameplayLootService = gameplayLootService;
            _lootFactory = lootFactory;

            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.LootSpawner,
                    GameMatcher.ConveyorSpawner,
                    GameMatcher.LootSpawnInterval,
                    GameMatcher.CooldownUp
                ));

            _activeLoot = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Loot,
                    GameMatcher.View
                ));
        }

        public void Execute()
        {
            foreach (var spawner in _entities.GetEntities(_buffer))
            {
                if (_activeLoot.GetEntities().Length >= LootStaticData.MaxLootAmount)
                    continue;

                if (_currentConfig >= _gameplayLootService.AvailableLoot.Count)
                    _currentConfig = 0;

                LootSettingsData lootSetup = _gameplayLootService.AvailableLoot[_currentConfig];
                Transform spawn = GetSpawnPoint();
                _lootFactory.CreateLootEntity(lootSetup.Type, _provider.Context.LootParent, spawn.position, spawn.rotation.eulerAngles);
                _currentConfig++;

                spawner.PutOnCooldown(spawner.LootSpawnInterval);
            }
        }

        private Transform GetSpawnPoint()
        {
            var spawnPosition = _provider.Context.LootSpawnPoints[0];
            return spawnPosition;
        }
    }
}