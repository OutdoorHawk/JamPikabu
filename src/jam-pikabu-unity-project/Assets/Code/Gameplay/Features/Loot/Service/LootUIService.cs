using System;
using System.Collections.Generic;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;

namespace Code.Gameplay.Features.Loot.Service
{
    public class LootUIService : ILootUIService
    {
        public event Action OnLootUpdate;
        public event Action<LootTypeId> OnLootItemAdded;

        private readonly List<LootTypeId> _collectedLootItems = new();
        private readonly List<LootTypeId> _consumedLoot = new();

        public IReadOnlyList<LootTypeId> CollectedLootItems => _collectedLootItems;

        public void CreateNewCollectedLootItem(LootTypeId lootType)
        {
            _collectedLootItems.Add(lootType);
            NotifyLootItemAdded(lootType);
            NotifyLootUpdated();
        }

        public void AddConsumedLoot(LootTypeId lootType)
        {
            _consumedLoot.Add(lootType);
        }

        public void ClearCollectedLoot()
        {
            _collectedLootItems.Clear();
            NotifyLootUpdated();
        }

        private void NotifyLootItemAdded(LootTypeId lootType)
        {
            OnLootItemAdded?.Invoke(lootType);
        }


        private void NotifyLootUpdated()
        {
            OnLootUpdate?.Invoke();
        }
    }
}