﻿using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Loot.Data;
using Code.Infrastructure.Common;

namespace Code.Gameplay.Features.Loot.Service
{
    public interface IGameplayLootService
    {
        event Action OnLootUpdate;
        bool LootIsBusy { get; }
        int MaxExtraLootAmount { get; }
        IReadOnlyList<LootTypeId> CollectedLootItems { get; }
        CircularList<LootSettingsData> AvailableIngredients { get; }
        CircularList<LootSettingsData> AvailableExtraLoot { get; }
        IReadOnlyList<CollectedLootData> CollectedLoot { get; }
        void CreateLootSpawner();
        void TrySpawnIngredientLoot();
        void TrySpawnExtraLoot();
        void SpawnLoot(LootTypeId type);
        void AddCollectedLoot(LootTypeId lootType, int ratingAmount);
        void SetLootIsConsumingState(bool state);
        void ClearCollectedLoot();
        void CreateLootConsumer();
        void TrySpawnConsumableLoot();
        void DayEnd();
    }
}