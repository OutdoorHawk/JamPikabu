using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Meta.Features.LootCollection.Configs;

namespace Code.Meta.Features.LootCollection.Service
{
    public interface ILootCollectionService
    {
        event Action OnUpgraded;
        event Action OnFreeUpgradeTimeEnd;
        event Action OnNewLootUnlocked;
        Dictionary<LootTypeId, LootItemCollectionData> LootProgression { get; }
        LootProgressionStaticData LootData { get; }
        void InitializeLootProgression(List<LootItemCollectionData> items);
        void AddNewUnlockedLoot(LootTypeId type);
        void LootUpgraded(LootTypeId type, int newLevel);
        void FreeUpgradeTimerUpdated(LootTypeId type, int nextTime);
        bool UpgradedForMaxLevel(LootTypeId type);
        bool CanUpgradeForFree(LootTypeId type);
        bool TimeToFreeUpgradePassed(LootTypeId type);
        int GetTimeLeftToFreeUpgrade(LootTypeId type);
        bool TryGetLootLevel(LootTypeId type, out LootLevelData levelData);
        
    }
}