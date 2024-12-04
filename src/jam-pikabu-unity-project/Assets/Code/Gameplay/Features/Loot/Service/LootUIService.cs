using System;
using System.Collections.Generic;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;

namespace Code.Gameplay.Features.Loot.Service
{
    public class LootUIService : ILootUIService, IEnterGameLoopStateHandler
    {
        public event Action OnLootUpdate;
        public event Action<LootTypeId> OnLootItemAdded;

        private readonly List<LootTypeId> _collectedLootItems = new();

        public IReadOnlyList<LootTypeId> CollectedLootItems => _collectedLootItems;

        #region IEnterGameLoopStateHandler

        public OrderType OrderType { get; }


        public void OnEnterGameLoop()
        {
            //todo clear on start every turn
            _collectedLootItems?.Clear();
        }

        #endregion

        public void CreateNewCollectedLootItem(LootTypeId lootType)
        {
            _collectedLootItems.Add(lootType);
            NotifyLootItemAdded(lootType);
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