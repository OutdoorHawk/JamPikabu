using System.Collections.Generic;
using System.Threading;
using Code.Common;
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
    public class SpawnLootSystem : ReactiveSystem<GameEntity>, ITearDownSystem
    {
        private readonly ILootFactory _lootFactory;
        private readonly IStaticDataService _staticDataService;
        private readonly ISceneContextProvider _provider;
        private readonly GameContext _context;

        private readonly CancellationTokenSource _exitGameSource = new();
        private int _spawnPointIndex;

        public SpawnLootSystem(GameContext context, ILootFactory lootFactory, IStaticDataService staticDataService, ISceneContextProvider provider) :
            base(context)
        {
            _context = context;
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
            foreach (var lootSpawner in entities)
            {
                SpawnLootAsync(lootSpawner).Forget();
            }
        }

        private async UniTaskVoid SpawnLootAsync(GameEntity lootSpawner)
        {
            int lootSpawnerId = lootSpawner.Id;
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

            GameEntity spawner = _context.GetEntityWithId(lootSpawnerId);

            if (spawner.IsNullOrDestructed())
                return;
            
            spawner.isDestructed = true;
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