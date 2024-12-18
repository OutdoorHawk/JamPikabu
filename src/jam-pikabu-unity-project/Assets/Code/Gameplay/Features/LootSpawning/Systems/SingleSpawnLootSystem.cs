using System.Collections.Generic;
using System.Threading;
using Code.Common;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Factory;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.StaticData;
using Code.Infrastructure.SceneContext;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.LootSpawning.Systems
{
    public class SingleSpawnLootSystem : ReactiveSystem<GameEntity>, ITearDownSystem, ICleanupSystem
    {
        private readonly ILootFactory _lootFactory;
        private readonly IStaticDataService _staticDataService;
        private readonly ISceneContextProvider _provider;
        private readonly IGameplayLootService _gameplayLootService;
        private readonly GameContext _context;
        
        private readonly IGroup<GameEntity> _completedSpawners;

        private readonly CancellationTokenSource _exitGameSource = new();

        private int _spawnPointIndex;

        public SingleSpawnLootSystem(GameContext context, ILootFactory lootFactory, IStaticDataService staticDataService,
            ISceneContextProvider provider, IGameplayLootService gameplayLootService) :
            base(context)
        {
            _context = context;
            _provider = provider;
            _gameplayLootService = gameplayLootService;
            _staticDataService = staticDataService;
            _lootFactory = lootFactory;

            _completedSpawners = context.GetGroup(GameMatcher.AllOf(
                GameMatcher.LootSpawner,
                GameMatcher.SingleSpawn,GameMatcher.Complete));
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(
                GameMatcher.LootSpawner,
                GameMatcher.SingleSpawn).Added());
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

        public void TearDown()
        {
            _exitGameSource?.Cancel();
        }

        public void Cleanup()
        {
            foreach (GameEntity completedSpawner in _completedSpawners)
            {
                completedSpawner.isDestructed = true;
            }
        }

        private async UniTaskVoid SpawnLootAsync(GameEntity lootSpawner)
        {
            int lootSpawnerId = lootSpawner.Id;
            var staticData = _staticDataService.GetStaticData<LootSettingsStaticData>();

            SceneContextComponent sceneContext = _provider.Context;

            await DelaySeconds(staticData.LootSpawnStartDelay, _exitGameSource.Token);

            await ProcessLootSpawn(staticData, sceneContext);
            
            await DelaySeconds(staticData.DelayAfterLootSpawn, _exitGameSource.Token);

            GameEntity spawner = _context.GetEntityWithId(lootSpawnerId);

            if (spawner.IsNullOrDestructed())
                return;

            spawner.isComplete = true;
        }

        private async UniTask ProcessLootSpawn(LootSettingsStaticData staticData, SceneContextComponent sceneContext)
        {
            float lootSpawnAmount = staticData.MaxLootAmount;

            if (lootSpawnAmount == 0 || _gameplayLootService.AvailableLoot == null)
                return;

            for (int i = 0; i < lootSpawnAmount / _gameplayLootService.AvailableLoot.Count; i++)
            {
                Transform spawn = GetSpawnPoint(sceneContext);

                foreach (var lootSetup in _gameplayLootService.AvailableLoot)
                {
                    _lootFactory.CreateLootEntity(lootSetup.Type, sceneContext.LootParent, spawn.position, spawn.rotation.eulerAngles);
                    await DelaySeconds(staticData.LootSpawnInterval, _exitGameSource.Token);
                }
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
    }
}