using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot.Configs;

namespace Code.Gameplay.Features.Loot.Service
{
    public interface IGameplayLootService
    {
        event Action OnLootUpdate;
        bool LootIsBusy { get; }
        IReadOnlyList<LootTypeId> CollectedLootItems { get; }
        IReadOnlyList<LootSetup> AvailableLoot { get; }
        void CreateLootSpawner();
        void AddCollectedLoot(LootTypeId lootType);
        void SetLootIsConsumingState(bool state);
        void ClearCollectedLoot();
        void CreateLootConsumer();
    }
}