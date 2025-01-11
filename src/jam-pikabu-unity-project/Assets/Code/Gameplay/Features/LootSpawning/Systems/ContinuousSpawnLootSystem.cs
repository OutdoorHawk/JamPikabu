using System.Collections.Generic;
using System.Linq;
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
        private readonly IGameplayLootService _gameplayLootService;
        private readonly ILootFactory _lootFactory;
        private readonly IGroup<GameEntity> _entities;
        private readonly IGroup<GameEntity> _activeLoot;
        private readonly List<GameEntity> _buffer = new(2);

        private int _currentConfig;
        private int _spawnPointIndex;

        private LootSettingsStaticData LootStaticData => _staticDataService.Get<LootSettingsStaticData>();

        public ContinuousSpawnLootSystem(GameContext context, IStaticDataService staticDataService,
            ISceneContextProvider provider, IGameplayLootService gameplayLootService, ILootFactory lootFactory)
        {
            _staticDataService = staticDataService;
            _provider = provider;
            _gameplayLootService = gameplayLootService;
            _lootFactory = lootFactory;

            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.LootSpawner,
                    GameMatcher.ContinuousSpawn,
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
                if (_activeLoot.count >= LootStaticData.MaxLootAmount)
                {
                    spawner.isDestructed = true;
                    continue;
                }

                if (_currentConfig >= _gameplayLootService.AvailableLoot.Count)
                    _currentConfig = 0;

                LootSettingsData lootSetup = _gameplayLootService.AvailableLoot[_currentConfig];
                _currentConfig++;
                
                if (CheckSpawnChance(lootSetup) == false)
                    continue;
                
                if (CheckTypeMaxAmountReached(lootSetup))
                    continue;
                
                Transform spawn = GetSpawnPoint();
                _lootFactory.CreateLootEntity(lootSetup.Type, _provider.Context.LootParent, spawn.position, spawn.rotation.eulerAngles);

                spawner.PutOnCooldown(spawner.LootSpawnInterval);
            }
        }

        private bool CheckSpawnChance(LootSettingsData lootSetup)
        {
            if (lootSetup.SpawnChance == 100)
                return true;

            int value = Random.Range(0, 101);
            
            if (lootSetup.SpawnChance >= value)
                return true;

            return false;
        }
        
        private bool CheckTypeMaxAmountReached(LootSettingsData lootSetup)
        {
            if (lootSetup.SpawnChance != 100)
                return false;
            
            float typeMaxAmount = LootStaticData.MaxLootAmount / _gameplayLootService.AvailableLoot.Count;

            List<GameEntity> typedLoot = _activeLoot
                .GetEntities()
                .ToList()
                .FindAll(x => x.LootTypeId == lootSetup.Type);

            if (typedLoot.Count >= typeMaxAmount)
                return true;

            return false;
        }

        private Transform GetSpawnPoint()
        {
            Transform[] spawnPoints = _provider.Context.LootSpawnPoints;
            var spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)];
            return spawnPosition;
        }
    }
}