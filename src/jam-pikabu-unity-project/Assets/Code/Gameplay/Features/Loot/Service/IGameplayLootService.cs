using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Data;

namespace Code.Gameplay.Features.Loot.Service
{
    public interface IGameplayLootService
    {
        event Action OnLootUpdate;
        bool LootIsBusy { get; }
        IReadOnlyList<LootTypeId> CollectedLootItems { get; }
        IReadOnlyList<LootSettingsData> AvailableLoot { get; }
        IReadOnlyList<CollectedLootData> CollectedLoot { get; }
        void CreateLootSpawner();
        void AddCollectedLoot(LootTypeId lootType, int ratingAmount);
        void SetLootIsConsumingState(bool state);
        void ClearCollectedLoot();
        void CreateLootConsumer();
    }
}