using System;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.StaticData;
using Code.Infrastructure.SceneLoading;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.LootCollection;
using Code.Meta.Features.LootCollection.Configs;
using Code.Meta.Features.LootCollection.Service;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Factory
{
    public class LootFactory : ILootFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IDaysService _daysService;
        private readonly ILootCollectionService _lootCollectionService;

        public LootFactory(IStaticDataService staticDataService, IDaysService daysService, ILootCollectionService lootCollectionService)
        {
            _staticDataService = staticDataService;
            _daysService = daysService;
            _lootCollectionService = lootCollectionService;
        }

        public GameEntity CreateLootSpawner()
        {
            DayData dayData = _daysService.GetDayData();

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

            switch (typeId)
            {
                case LootTypeId.GoldCoin:
                    loot.AddGold(1);
                    break;
                default:
                    AddRating(loot, typeId);
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
            var staticData = _staticDataService.GetStaticData<LootSettingsStaticData>();

            GameEntity loot = CreateGameEntity.Empty()
                    .With(x => x.isLoot = true)
                    .AddStartWorldPosition(at)
                    .AddStartRotation(spawnRotation)
                    .AddTargetParent(parent)
                    .AddLootTypeId(typeId)
                    .With(x => x.AddViewPrefab(staticData.LootItem), when: lootSetup.ViewPrefab == null)
                    .With(x => x.AddViewPrefab(lootSetup.ViewPrefab), when: lootSetup.ViewPrefab != null)
                ;

            return loot;
        }

        private void AddRating(GameEntity loot, LootTypeId typeId)
        {
            LootSetup lootSetup = GetLootSetup(typeId);
            
            loot.AddBaseRating(lootSetup.BaseRatingValue)
                .With(x => x.isConsumableIngredient = true);

            loot.AddRating(loot.BaseRating);

            if (_lootCollectionService.TryGetLootLevel(typeId, out var levelData))
            {
                loot.ReplaceRating(loot.BaseRating + levelData.RatingBoostAmount);
            }
        }

        private LootSetup GetLootSetup(LootTypeId typeId)
        {
            var staticData = _staticDataService.GetStaticData<LootSettingsStaticData>();
            var lootSetup = staticData.GetConfig(typeId);
            return lootSetup;
        }
    }
}