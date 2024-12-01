using System.Collections.Generic;
using System.Threading;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Factory;
using Code.Gameplay.StaticData;
using Code.Infrastructure.SceneContext;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.Loot.Systems
{
    public class SpawnLootSystem : IInitializeSystem, ITearDownSystem
    {
        private readonly ILootFactory _lootFactory;
        private readonly IStaticDataService _staticDataService;
        private readonly ISceneContextProvider _provider;

        private int _spawnPointIndex;

        private readonly CancellationTokenSource _exitGameSource = new();

        public SpawnLootSystem(ILootFactory lootFactory, IStaticDataService staticDataService, ISceneContextProvider provider)
        {
            _provider = provider;
            _staticDataService = staticDataService;
            _lootFactory = lootFactory;
        }

        public void Initialize()
        {
            SpawnLootAsync().Forget();
        }

        private async UniTaskVoid SpawnLootAsync()
        {
            GameEntity lootSpawner = CreateGameEntity.Empty()
                .With(x => x.isLootSpawner = true);

            lootSpawner.Retain(this);

            var staticData = _staticDataService.GetStaticData<LootStaticData>();
            List<LootSetup> configs = staticData.Configs;
            
            SceneContextComponent sceneContext = _provider.Context;

            for (int i = 0; i < staticData.LootSpawnAmount / configs.Count; i++)
            {
                Transform spawn = GetSpawnPoint(sceneContext);

                foreach (var lootSetup in configs)
                {
                    _lootFactory.CreateLootEntity(lootSetup.Type, sceneContext.LootParent, spawn.position, spawn.rotation.eulerAngles);
                    await DelaySeconds(staticData.LootSpawnInterval, _exitGameSource.Token);
                }
            }

            lootSpawner.Release(this);
            lootSpawner.isDestructed = true;
        }

        private Transform GetSpawnPoint(SceneContextComponent sceneContext)
        {
            if (_spawnPointIndex >= sceneContext.LootSpawnPoints.Length) 
                _spawnPointIndex = 0;
            
            var spawnPosition = sceneContext.LootSpawnPoints[_spawnPointIndex];
            _spawnPointIndex++;
            return spawnPosition;
        }

        public void TearDown()
        {
            _exitGameSource?.Cancel();
        }
    }
}