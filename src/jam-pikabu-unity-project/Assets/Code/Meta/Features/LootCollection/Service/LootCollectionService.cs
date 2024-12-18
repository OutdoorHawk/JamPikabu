using System;
using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.Loot;

namespace Code.Meta.Features.LootCollection.Service
{
    public class LootCollectionService : ILootCollectionService
    {
        private readonly ITimeService _timeService;
        public event Action OnUpgraded;
        public Dictionary<LootTypeId, LootItemCollectionData> LootProgression { get; private set; } = new();

        public LootCollectionService(ITimeService timeService)
        {
            _timeService = timeService;
        }

        public void InitializeLootProgression(List<LootItemCollectionData> items)
        {
            LootProgression.Clear();

            foreach (LootItemCollectionData item in items)
            {
                LootProgression.Add(item.Type, item);
            }
        }

        public void LootUpgraded(LootTypeId type, int newLevel)
        {
            LootProgression[type] = new LootItemCollectionData(type, newLevel);
            OnUpgraded?.Invoke();
        }

        public bool CanUpgradeForFree(LootTypeId type)
        {
            if (LootProgression.TryGetValue(type, out var progression) == false)
                return false;

            if (progression.FreeUpgradeTime == 0)
                return false;

            if (_timeService.Time < progression.FreeUpgradeTime)
                return false;

            return true;
        }

        public int GetTimeLeftToFreeUpgrade(LootTypeId type)
        {
            if (LootProgression.TryGetValue(type, out var progression) == false)
                return 0;

            if (progression.FreeUpgradeTime == 0)
                return 0;

            int diff = progression.FreeUpgradeTime - _timeService.Time;
            return diff.ZeroIfNegative();
        }
    }
}