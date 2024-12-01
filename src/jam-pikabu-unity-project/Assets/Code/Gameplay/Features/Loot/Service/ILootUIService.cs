using System;
using System.Collections.Generic;

namespace Code.Gameplay.Features.Loot.Service
{
    public interface ILootUIService
    {
        event Action OnLootUpdate;
        event Action<LootTypeId> OnLootItemAdded;
        IReadOnlyList<LootTypeId> CollectedLootItems { get; }
        void CreateNewCollectedLootItem(LootTypeId lootType);
    }
}