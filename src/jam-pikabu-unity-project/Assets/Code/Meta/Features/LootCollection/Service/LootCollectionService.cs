using System;
using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData;
using Code.Meta.Features.DayLootSettings.Configs;
using Code.Meta.Features.LootCollection.Configs;
using Code.Meta.Features.LootCollection.Data;
using Code.Meta.Features.LootCollection.Factory;
using Code.Meta.Features.MapBlocks.Behaviours;

namespace Code.Meta.Features.LootCollection.Service
{
    public class LootCollectionService : ILootCollectionService
    {
        private readonly ITimeService _timeService;
        private readonly IStaticDataService _staticData;
        private readonly ILootCollectionFactory _factory;
        public event Action OnUpgraded;
        public event Action OnFreeUpgradeTimeEnd;
        public event Action OnNewLootUnlocked;
        public Dictionary<LootTypeId, LootLevelsProgressionData> LootLevels { get; private set; } = new();
        public Dictionary<LootTypeId, LootFreeUpgradeTimerData> LootFreeUpgrade { get; private set; } = new();

        public LootProgressionStaticData LootData => _staticData.Get<LootProgressionStaticData>();
        private MapBlocksStaticData MapBlocksStaticData => _staticData.Get<MapBlocksStaticData>();

        public LootCollectionService(ITimeService timeService,
            IStaticDataService staticData, ILootCollectionFactory factory)
        {
            _timeService = timeService;
            _staticData = staticData;
            _factory = factory;
        }

        public void InitializeLootProgression(List<LootLevelsProgressionData> items)
        {
            LootLevels.Clear();

            foreach (LootLevelsProgressionData item in items)
            {
                LootLevels.Add(item.Type, item);
            }
        }

        public void InitializeLootFreeUpgradeTimers(List<LootFreeUpgradeTimerData> items)
        {
            LootFreeUpgrade.Clear();

            foreach (var item in items)
            {
                LootFreeUpgrade.Add(item.Type, item);
            }
        }

        public void AddNewUnlockedLoot(LootTypeId type)
        {
            LootLevels.Add(type, new LootLevelsProgressionData(type, 0));
            OnNewLootUnlocked?.Invoke();
        }

        public void LootUpgraded(LootTypeId type, int newLevel)
        {
            LootLevelsProgressionData currentData = LootLevels[type];
            currentData.Level = newLevel;
            LootLevels[type] = currentData;
            OnUpgraded?.Invoke();
        }

        public bool UpgradedForMaxLevel(LootTypeId type)
        {
            if (LootLevels.TryGetValue(type, out var progression) == false)
                return false;

            LootProgressionData progressionData = LootData.GetConfig(type);

            if (progressionData == null)
                return false;

            bool result = progression.Level >= progressionData.Levels.Count;
            return result;
        }

        public bool CanUpgradeForFree(LootTypeId type)
        {
            LootProgressionData progressionData = LootData.GetConfig(type);

            if (progressionData == null)
                return false;

            MapBlockData mapBlockData = MapBlocksStaticData.GetMapBlockDataByLinkedIngredient(type);

            if (mapBlockData.FreeUpgradeTimeMinutes == 0)
                return false;

            return true;
        }

        public bool TimeToFreeUpgradePassed(LootTypeId type)
        {
            bool result = GetTimeLeftToFreeUpgrade(type) <= 0;
            return result;
        }

        public int GetTimeLeftToFreeUpgrade(LootTypeId type)
        {
            if (LootFreeUpgrade.TryGetValue(type, out var progression) == false)
                return 0;

            if (progression.NextFreeUpgradeTimeStamp == 0)
                return 0;

            int diff = progression.NextFreeUpgradeTimeStamp - _timeService.TimeStamp;
            return diff.ZeroIfNegative();
        }

        public bool TryGetLootLevel(LootTypeId type, out LootLevelData levelData)
        {
            levelData = null;

            if (LootLevels.TryGetValue(type, out LootLevelsProgressionData lootProgression) == false)
                return false;

            LootProgressionData progressionStatic = LootData.GetConfig(type);

            if (progressionStatic == null)
                return false;

            if (lootProgression.Level >= progressionStatic.Levels.Count)
            {
                levelData = progressionStatic.Levels[^1];
                return true;
            }

            levelData = progressionStatic.Levels[lootProgression.Level];
            return true;
        }

        public void CreateFreeUpgradeTimer(LootTypeId type)
        {
            MapBlockData mapBlockData = MapBlocksStaticData.GetMapBlockDataByLinkedIngredient(type);

            if (mapBlockData == null)
                return;

            if (mapBlockData.FreeUpgradeTimeMinutes == 0)
                return;

            MetaEntity timer = _factory.CreateLootFreeUpgradeTimer(type, mapBlockData.FreeUpgradeTimeSeconds);
            FreeUpgradeTimerUpdated(type, timer.NextFreeUpgradeTime);
        }

        public void FreeUpgradeTimerEnded(LootTypeId type)
        {
            FreeUpgradeTimerUpdated(type, 0);
        }
        
        private void FreeUpgradeTimerUpdated(LootTypeId type, int nextTime)
        {
            if (LootFreeUpgrade.TryGetValue(type, out var currentData))
            {
                currentData.NextFreeUpgradeTimeStamp = nextTime;
                LootFreeUpgrade[type] = currentData;
            }
            else
            {
                LootFreeUpgrade.Add(type, new LootFreeUpgradeTimerData(type, nextTime));
            }

            OnFreeUpgradeTimeEnd?.Invoke();
        }
    }
}