using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Meta.Features.LootCollection.Configs;
using Code.Meta.Features.LootCollection.Data;

namespace Code.Meta.Features.LootCollection.Service
{
    public interface ILootCollectionService
    {
        event Action OnUpgraded;
        event Action OnFreeUpgradeTimeEnd;
        event Action OnNewLootUnlocked;
        Dictionary<LootTypeId, LootLevelsProgressionData> LootLevels { get; }
        Dictionary<LootTypeId, LootFreeUpgradeTimerData> LootFreeUpgrade { get; }
        void InitializeLootProgression(List<LootLevelsProgressionData> items);
        void InitializeLootFreeUpgradeTimers(List<LootFreeUpgradeTimerData> items);
        void AddNewUnlockedLoot(LootTypeId type);
        void LootUpgraded(LootTypeId type, int newLevel);
        bool UpgradedForMaxLevel(LootTypeId type);
        bool CanUpgradeForFree(LootTypeId type);
        bool TimeToFreeUpgradePassed(LootTypeId type);
        int GetTimeLeftToFreeUpgrade(LootTypeId type);
        bool TryGetLootLevel(LootTypeId type, out LootLevelData levelData);
        void CreateFreeUpgradeTimer(LootTypeId type);
        void FreeUpgradeTimerEnded(LootTypeId type);
    }
}