using System.Collections.Generic;
using System.Threading;
using Code.Common;
using Code.Common.Extensions;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Factory;
using Code.Gameplay.Features.RoundState.Service;
using Code.Gameplay.StaticData;
using Code.Infrastructure.SceneContext;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.LootSpawning.Systems
{
    public class SpawnLootSystem : ReactiveSystem<GameEntity>, ITearDownSystem
    {
        private readonly ILootFactory _lootFactory;
        private readonly IStaticDataService _staticDataService;
        private readonly ISceneContextProvider _provider;
        private readonly IRoundStateService _roundStateService;
        private readonly GameContext _context;

        private readonly CancellationTokenSource _exitGameSource = new();
        private readonly List<LootSetup> _lootSetupsBuffer = new();
        
        private int _spawnPointIndex;

        public SpawnLootSystem(GameContext context, ILootFactory lootFactory, IStaticDataService staticDataService,
            ISceneContextProvider provider, IRoundStateService roundStateService) :
            base(context)
        {
            _context = context;
            _provider = provider;
            _roundStateService = roundStateService;
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
            int currentDay = _roundStateService.CurrentDay;

            InitLootBuffer(configs, currentDay);

            SceneContextComponent sceneContext = _provider.Context;

            await AsyncGameplayExtensions.DelaySeconds(staticData.LootSpawnStartDelay, _exitGameSource.Token);

            await ProcessLootSpawn(staticData, sceneContext);

            GameEntity spawner = _context.GetEntityWithId(lootSpawnerId);

            if (spawner.IsNullOrDestructed())
                return;

            spawner.isDestructed = true;
        }

        private void InitLootBuffer(List<LootSetup> configs, int currentDay)
        {
            _lootSetupsBuffer.Clear();
            foreach (var config in configs)
            {
                if (CheckMinDayToUnlock(config, currentDay))
                    continue;

                if (CheckMaxDayToUnlock(config, currentDay))
                    continue;

                _lootSetupsBuffer.Add(config);
            }
        }

        private async UniTask ProcessLootSpawn(LootStaticData staticData, SceneContextComponent sceneContext)
        {
            for (int i = 0; i < staticData.LootSpawnAmount / _lootSetupsBuffer.Count; i++)
            {
                Transform spawn = GetSpawnPoint(sceneContext);

                foreach (var lootSetup in _lootSetupsBuffer)
                {
                    _lootFactory.CreateLootEntity(lootSetup.Type, sceneContext.LootParent, spawn.position, spawn.rotation.eulerAngles);
                    await AsyncGameplayExtensions.DelaySeconds(staticData.LootSpawnInterval, _exitGameSource.Token);
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

        public void TearDown()
        {
            _exitGameSource?.Cancel();
        }

        private static bool CheckMinDayToUnlock(LootSetup data, int currentDay)
        {
            return data.MinDayToUnlock > 0 && currentDay < data.MinDayToUnlock;
        }

        private static bool CheckMaxDayToUnlock(LootSetup data, int currentDay)
        {
            return data.MaxDayToUnlock > 0 && currentDay > data.MaxDayToUnlock;
        }
    }
}