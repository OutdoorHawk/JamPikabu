using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot;

namespace Code.Meta.Features.LootCollection.Service
{
    public interface ILootCollectionService
    {
        event Action OnUpgraded;
        event Action OnFreeUpgradeTimeEnd;
        Dictionary<LootTypeId, LootItemCollectionData> LootProgression { get; }
        void InitializeLootProgression(List<LootItemCollectionData> items);
        void AddNewUnlockedLoot(LootTypeId type);
        void LootUpgraded(LootTypeId type, int newLevel);
        void FreeUpgradeTimerUpdated(LootTypeId type, int nextTime);
        bool UpgradedForMaxLevel(LootTypeId type);
        bool CanUpgradeForFree(LootTypeId type);
        bool TimeToFreeUpgradePassed(LootTypeId type);
        int GetTimeLeftToFreeUpgrade(LootTypeId type);
    }
}