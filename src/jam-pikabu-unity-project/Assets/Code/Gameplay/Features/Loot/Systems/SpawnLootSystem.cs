using System.Collections.Generic;
using System.Threading;
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
    public class SpawnLootSystem : ReactiveSystem<GameEntity>
    {
        private readonly ILootFactory _lootFactory;
        private readonly IStaticDataService _staticDataService;
        private readonly ISceneContextProvider _provider;

        private CancellationTokenSource _exitGameSource;
        private int _spawnPointIndex;

        public SpawnLootSystem(GameContext context, ILootFactory lootFactory, IStaticDataService staticDataService, ISceneContextProvider provider) :
            base(context)
        {
            _provider = provider;
            _staticDataService = staticDataService;
            _lootFactory = lootFactory;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.LootSpawner.Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return true;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            SpawnLootAsync(entities).Forget();
        }

        private async UniTaskVoid SpawnLootAsync(List<GameEntity> lootSpawners)
        {
            foreach (var spawner in lootSpawners)
                spawner.Retain(this);

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

            foreach (var spawner in lootSpawners)
            {
                spawner.Release(this);
                spawner.isDestructed = true;
                ;
            }
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