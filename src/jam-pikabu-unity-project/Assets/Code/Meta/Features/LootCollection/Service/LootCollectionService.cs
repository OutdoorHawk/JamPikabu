using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot;

namespace Code.Meta.Features.LootCollection.Service
{
    public class LootCollectionService : ILootCollectionService
    {
        public event Action OnUpgraded;
        public Dictionary<LootTypeId, LootItemCollectionData> LootProgression { get; private set; } = new();
        
        public void InitializeLootProgression(List<LootItemCollectionData> items)
        {
            LootProgression.Clear();
            
            foreach (LootItemCollectionData item in items)
            {
                LootProgression.Add(item.Type, item);
            }
        }

        public void LootUpgraded(LootTypeId type, int newLevel)
        {
            LootProgression[type] = new LootItemCollectionData(type, newLevel);
            OnUpgraded?.Invoke();
        }
    }
}