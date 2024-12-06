using System;
using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.RoundState.Configs;
using Code.Gameplay.Features.RoundState.Service;
using Code.Gameplay.StaticData;
using Code.Infrastructure.SceneLoading;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Factory
{
    public class LootFactory : ILootFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IRoundStateService _roundStateService;

        public LootFactory(IStaticDataService staticDataService, IRoundStateService roundStateService)
        {
            _staticDataService = staticDataService;
            _roundStateService = roundStateService;
        }

        public GameEntity CreateLootSpawner()
        {
            DayData dayData = _roundStateService.GetDayData();
            
            switch (dayData.SceneId)
            {
                case SceneTypeId.Level_1:
                   return CreateSingleSpawner();
                case SceneTypeId.Level_2:
                    return CreateConveyorSpawner();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public GameEntity CreateLootEntity(LootTypeId typeId, Transform parent, Vector2 at, Vector3 spawnRotation)
        {
            LootSetup lootSetup = GetLootSetup(typeId);
            GameEntity loot = CreateBaseLoot(typeId, parent, at, spawnRotation);

            AddEffects(loot, typeId);
            
            switch (typeId)
            {
                case LootTypeId.Unknown:
                    break;
            }

            return loot;
        }

        private static GameEntity CreateSingleSpawner()
        {
            return CreateGameEntity.Empty()
                .With(x => x.isLootSpawner = true)
                .With(x => x.isSingleSpawn = true)
                ;
        }

        private GameEntity CreateConveyorSpawner()
        {
            return CreateGameEntity.Empty()
                    .With(x => x.isLootSpawner = true)
                    .With(x => x.isConveyorSpawner = true)
                    .With(x => x.isCooldownUp = true)
                    .With(x => x.isComplete = true)
                ;
        }

        private GameEntity CreateBaseLoot(LootTypeId typeId, Transform parent, Vector2 at, Vector3 spawnRotation)
        {
            LootSetup lootSetup = GetLootSetup(typeId);

            GameEntity loot = CreateGameEntity.Empty()
                    .With(x => x.isLoot = true)
                    .AddStartWorldPosition(at)
                    .AddStartRotation(spawnRotation)
                    .AddTargetParent(parent)
                    .AddLootTypeId(typeId)
                    .AddViewPrefab(lootSetup.ViewPrefab)
                ;

            return loot;
        }

        private LootSetup GetLootSetup(LootTypeId typeId)
        {
            var staticData = _staticDataService.GetStaticData<LootStaticData>();
            var lootSetup = staticData.GetConfig(typeId);
            return lootSetup;
        }

        private void AddEffects(GameEntity loot, LootTypeId typeId)
        {
            LootSetup lootSetup = GetLootSetup(typeId);
        }
    }
}