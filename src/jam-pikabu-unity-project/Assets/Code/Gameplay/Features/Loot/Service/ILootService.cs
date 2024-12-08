using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot.Configs;

namespace Code.Gameplay.Features.Loot.Service
{
    public interface ILootService
    {
        event Action OnLootUpdate;
        event Action<LootTypeId> OnLootItemAdded;
        IReadOnlyList<LootTypeId> CollectedLootItems { get; }
        IReadOnlyList<LootSetup> AvailableLoot { get; }
        void InitLootBuffer();
        void CreateNewCollectedLootItem(LootTypeId lootType);
        void ClearCollectedLoot();
    }
}