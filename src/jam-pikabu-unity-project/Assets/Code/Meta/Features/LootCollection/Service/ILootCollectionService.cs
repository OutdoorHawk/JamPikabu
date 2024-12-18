using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot;

namespace Code.Meta.Features.LootCollection.Service
{
    public interface ILootCollectionService
    {
        event Action OnUpgraded;
        Dictionary<LootTypeId, LootItemCollectionData> LootProgression { get; }
        void InitializeLootProgression(List<LootItemCollectionData> items);
        void LootUpgraded(LootTypeId type, int newLevel);
        bool CanUpgradeForFree(LootTypeId type);
        int GetTimeLeftToFreeUpgrade(LootTypeId type);
    }
}