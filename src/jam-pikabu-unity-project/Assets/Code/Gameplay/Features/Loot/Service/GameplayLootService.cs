﻿using System;
using System.Collections.Generic;
using System.Linq;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Consumables.Config;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Data;
using Code.Gameplay.Features.Loot.Factory;
using Code.Gameplay.Features.LootSpawning.Factory;
using Code.Gameplay.Features.Result.Service;
using Code.Gameplay.StaticData;
using Code.Infrastructure.Common;
using Code.Infrastructure.SceneContext;
using Code.Meta.Features.BonusLevel.Config;
using Code.Meta.Features.Consumables.Service;
using Code.Meta.Features.DayLootSettings.Configs;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.LootCollection.Service;
using RoyalGold.Sources.Scripts.Game.MVC.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Gameplay.Features.Loot.Service
{
    public class GameplayLootService : IGameplayLootService
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IDaysService _daysService;
        private readonly ILootSpawnerFactory _lootSpawnerFactory;
        private readonly IConsumablesUIService _consumablesUIService;
        private readonly ILootFactory _lootFactory;
        private readonly ISceneContextProvider _provider;
        private readonly ILootCollectionService _lootCollection;
        private readonly IResultWindowService _resultWindowService;
        public event Action OnLootUpdate;

        private readonly List<LootTypeId> _collectedLootItems = new();
        private readonly List<CollectedLootData> _collectedLoot = new();
        private readonly CircularList<LootSettingsData> _availableIngredients = new();
        private readonly CircularList<LootSettingsData> _availableExtraLoot = new();
        private readonly List<ConsumablesData> _consumablesToSpawn = new();

        public bool LootIsBusy { get; private set; }
        public int MaxExtraLootAmount => _availableExtraLoot.Count * LootSettings.MaxEachExtraLootAmount;
        public IReadOnlyList<LootTypeId> CollectedLootItems => _collectedLootItems;
        public CircularList<LootSettingsData> AvailableIngredients => _availableIngredients;
        public CircularList<LootSettingsData> AvailableExtraLoot => _availableExtraLoot;
        public IReadOnlyList<CollectedLootData> CollectedLoot => _collectedLoot;
        private LootSettingsStaticData LootSettings => _staticDataService.Get<LootSettingsStaticData>();

        public GameplayLootService
        (
            IStaticDataService staticDataService,
            IDaysService daysService,
            ILootSpawnerFactory lootSpawnerFactory,
            IConsumablesUIService consumablesUIService,
            ILootFactory lootFactory,
            ISceneContextProvider provider,
            ILootCollectionService lootCollection,
            IResultWindowService resultWindowService
        )
        {
            _staticDataService = staticDataService;
            _daysService = daysService;
            _lootSpawnerFactory = lootSpawnerFactory;
            _consumablesUIService = consumablesUIService;
            _lootFactory = lootFactory;
            _provider = provider;
            _lootCollection = lootCollection;
            _resultWindowService = resultWindowService;
            ;
        }

        public void CreateLootSpawner()
        {
            RecalculateLootChances();
            InitLootBufferInternal();
            _lootSpawnerFactory.CreateLootSpawner();
        }

        public void TrySpawnIngredientLoot()
        {
            LootSettingsData lootSetup = AvailableIngredients.GetCurrent();
            CreateLoot(lootSetup);
        }

        public void TrySpawnExtraLoot()
        {
            LootSettingsData lootSetup = AvailableExtraLoot.GetCurrent();

            if (lootSetup == null)
                return;

            if (CheckSpawnChance(lootSetup.SpawnChance) == false)
                return;

            CreateLoot(lootSetup);
        }

        public void SpawnLoot(LootTypeId type)
        {
            LootSettingsData lootSetup = LootSettings.GetConfig(type);
            CreateLoot(lootSetup);
        }

        public void AddCollectedLoot(LootTypeId lootType, int ratingAmount)
        {
            _collectedLootItems.Add(lootType);
            _collectedLoot.Add(new CollectedLootData { Type = lootType, RatingAmount = ratingAmount });
            _resultWindowService.AddCollectedLoot(lootType);
            NotifyLootUpdated();
        }

        public void SetLootIsConsumingState(bool state)
        {
            LootIsBusy = state;
        }

        public void ClearCollectedLoot()
        {
            _collectedLootItems.Clear();
            _collectedLoot.Clear();

            NotifyLootUpdated();
        }

        public void CreateLootConsumer()
        {
            CreateGameEntity
                .Empty()
                .With(x => x.isLootEffectsApplier = true)
                .With(x => x.isAvailable = true)
                ;
        }

        public void TrySpawnConsumableLoot()
        {
            if (_consumablesToSpawn.Count == 0)
                return;

            foreach (ConsumablesData data in _consumablesToSpawn)
                SpawnLoot(data.LootTypeId);

            _consumablesToSpawn.Clear();
        }

        public void DayEnd()
        {
            _consumablesToSpawn.Clear();
        }

        private void RecalculateLootChances()
        {
            List<ConsumablesData> configs = _consumablesUIService.ConsumablesStaticData.Configs;

            foreach (ConsumablesData data in configs)
            {
                if (data.LevelNeedToUnlockSpawn > 0 && _daysService.CurrentDay < data.LevelNeedToUnlockSpawn)
                    continue;

                if (CheckSpawnChance(data.SpawnChanceInOrder) == false)
                    continue;

                _consumablesToSpawn.Add(data);
            }
        }

        private void InitLootBufferInternal()
        {
            var staticData = _staticDataService.Get<LootSettingsStaticData>();
            var dayLootSettingsStaticData = _staticDataService.Get<MapBlocksStaticData>();
            var currentDay = _daysService.GetDayData();
            _availableIngredients.Clear();

            switch (_daysService.BonusLevelType)
            {
                case BonusLevelType.GoldenCoins:
                {
                    FillBonusLevelLootBuffer(staticData);
                    break;
                }
                default:
                {
                    FillDefaultLootBuffer(dayLootSettingsStaticData, currentDay, staticData);
                    break;
                }
            }

            FallbackRandom(staticData);
        }

        private void FillBonusLevelLootBuffer(LootSettingsStaticData staticData)
        {
            BonusLevelData bonusLevelData = _staticDataService.Get<BonusLevelStaticData>().Configs[0];
            foreach (LootTypeId typeId in bonusLevelData.AvailableIngredients)
            {
                _availableIngredients.Add(staticData.GetConfig(typeId));
            }

            var lootLevels = _lootCollection.LootLevels.Values.ToList();
            
            if (lootLevels.Count <= 0) 
                return;
            
            lootLevels.ShuffleList();

            for (int i = 0; i < bonusLevelData.MainLevelsIngredientsAmount; i++)
            {
                if (i >= lootLevels.Count)
                    break;
                
                LootTypeId lootTypeId = lootLevels[i].Type;
                _availableIngredients.Add(staticData.GetConfig(lootTypeId));
            }
        }

        private void FillDefaultLootBuffer(MapBlocksStaticData dayLootSettingsStaticData, DayData currentDay, LootSettingsStaticData staticData)
        {
            MapBlockData mapBlock = dayLootSettingsStaticData.GetMapBlockDataByDayId(currentDay.Id);

            foreach (LootTypeId lootTypeId in mapBlock.AvailableIngredients)
            {
                _availableIngredients.Add(staticData.GetConfig(lootTypeId));
            }

            foreach (LootTypeId lootTypeId in mapBlock.ExtraLoot)
            {
                _availableExtraLoot.Add(staticData.GetConfig(lootTypeId));
            }
        }

        private void FallbackRandom(LootSettingsStaticData staticData)
        {
            if (_availableIngredients.Count != 0)
                return;

            List<LootTypeId> lootTypes = new List<LootTypeId>();

            for (LootTypeId i = 0; i < LootTypeId.Count; i++)
                lootTypes.Add(i);

            lootTypes.ShuffleList();

            for (int i = 0; i < 5; i++)
            {
                _availableIngredients.Add(staticData.GetConfig(lootTypes[i]));
            }
        }

        private void CreateLoot(LootSettingsData lootSetup)
        {
            Transform spawn = GetSpawnPoint();
            _lootFactory.CreateLootEntity(lootSetup.Type, _provider.Context.LootParent, spawn.position, spawn.rotation.eulerAngles);
        }

        private Transform GetSpawnPoint()
        {
            Transform[] spawnPoints = _provider.Context.LootSpawnPoints;
            var spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)];
            return spawnPosition;
        }

        private void NotifyLootUpdated()
        {
            OnLootUpdate?.Invoke();
        }

        private static bool CheckSpawnChance(int spawnChance)
        {
            if (spawnChance == 100)
                return true;

            int value = Random.Range(0, 101);

            if (spawnChance >= value)
                return true;

            return false;
        }
    }
}