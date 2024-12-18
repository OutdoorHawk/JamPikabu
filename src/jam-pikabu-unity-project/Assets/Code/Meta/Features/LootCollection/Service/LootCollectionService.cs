using System;
using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData;
using Code.Meta.Features.LootCollection.Configs;

namespace Code.Meta.Features.LootCollection.Service
{
    public class LootCollectionService : ILootCollectionService
    {
        private readonly ITimeService _timeService;
        private readonly IStaticDataService _staticData;
        public event Action OnUpgraded;
        public event Action OnFreeUpgradeTimeEnd;
        public Dictionary<LootTypeId, LootItemCollectionData> LootProgression { get; private set; } = new();
        
        private LootProgressionStaticData LootData => _staticData.GetStaticData<LootProgressionStaticData>();

        public LootCollectionService(ITimeService timeService, IStaticDataService staticData)
        {
            _timeService = timeService;
            _staticData = staticData;
        }

        public void InitializeLootProgression(List<LootItemCollectionData> items)
        {
            LootProgression.Clear();

            foreach (LootItemCollectionData item in items)
            {
                LootProgression.Add(item.Type, item);
            }
        }

        public void AddNewUnlockedLoot(LootTypeId type)
        {
            LootProgression.Add(type, new LootItemCollectionData(type, 1));
        }

        public void LootUpgraded(LootTypeId type, int newLevel)
        {
            LootItemCollectionData currentData = LootProgression[type];
            currentData.Level = newLevel;
            LootProgression[type] = currentData;
            OnUpgraded?.Invoke();
        }
        
        public void FreeUpgradeTimerUpdated(LootTypeId type, int nextTime)
        {
            LootItemCollectionData currentData = LootProgression[type];
            currentData.NextFreeUpgradeTime = nextTime;
            LootProgression[type] = currentData;
            OnFreeUpgradeTimeEnd?.Invoke();
        }

        public bool UpgradedForMaxLevel(LootTypeId type)
        {
            if (LootProgression.TryGetValue(type, out var progression) == false)
                return false;

            LootProgressionData progressionData = LootData.GetConfig(type);
            
            if (progressionData == null)
                return false;
            
            bool result = progression.Level >= progressionData.Levels.Count;
            return result;
        }

        public bool CanUpgradeForFree(LootTypeId type)
        {
            if (LootProgression.TryGetValue(type, out var progression) == false)
                return false;

            if (progression.NextFreeUpgradeTime == 0)
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
            if (LootProgression.TryGetValue(type, out var progression) == false)
                return 0;

            if (progression.NextFreeUpgradeTime == 0)
                return 0;

            int diff = progression.NextFreeUpgradeTime - _timeService.TimeStamp;
            return diff.ZeroIfNegative();
        }
    }
}