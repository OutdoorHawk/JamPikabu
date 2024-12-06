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
    public class ContinuousSpawnLootSystem : IExecuteSystem
    {
        private readonly IStaticDataService _staticDataService;
        private readonly ISceneContextProvider _provider;
        private readonly ILootService _lootService;
        private readonly ILootFactory _lootFactory;
        private readonly IGroup<GameEntity> _entities;
        private readonly IGroup<GameEntity> _activeLoot;
        private readonly List<GameEntity> _buffer = new(2);

        private int _currentConfig;

        private LootStaticData LootStaticData => _staticDataService.GetStaticData<LootStaticData>();

        public ContinuousSpawnLootSystem(GameContext context, IStaticDataService staticDataService,
            ISceneContextProvider provider, ILootService lootService, ILootFactory lootFactory)
        {
            _staticDataService = staticDataService;
            _provider = provider;
            _lootService = lootService;
            _lootFactory = lootFactory;

            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.LootSpawner,
                    GameMatcher.ConveyorSpawner,
                    GameMatcher.CooldownUp
                ));

            _activeLoot = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Loot,
                    GameMatcher.View
                ).NoneOf(
                    GameMatcher.Collected));
        }

        public void Execute()
        {
            foreach (var entity in _entities.GetEntities(_buffer))
            {
                LootStaticData staticData = LootStaticData;

                if (_activeLoot.GetEntities().Length >= LootStaticData.MaxLootAmount)
                    continue;

                if (_currentConfig >= _lootService.AvailableLoot.Count)
                    _currentConfig = 0;

                LootSetup lootSetup = _lootService.AvailableLoot[_currentConfig];
                Transform spawn = GetSpawnPoint();
                _lootFactory.CreateLootEntity(lootSetup.Type, _provider.Context.LootParent, spawn.position, spawn.rotation.eulerAngles);
                _currentConfig++;

                entity.PutOnCooldown(staticData.LootSpawnConveyorInterval);
            }
        }

        private Transform GetSpawnPoint()
        {
            var spawnPosition = _provider.Context.LootSpawnPoints[0];
            return spawnPosition;
        }
    }
}