﻿using System.Collections.Generic;
using Code.Meta.Features.LootCollection.Data;
using Code.Meta.Features.LootCollection.Service;
using Entitas;

namespace Code.Meta.Features.LootCollection.Systems
{
    public class InitLootProgressionSystem : IInitializeSystem
    {
        private readonly ILootCollectionService _lootCollectionService;
        private readonly IGroup<MetaEntity> _lootCollection;

        public InitLootProgressionSystem(MetaContext context, ILootCollectionService lootCollectionService)
        {
            _lootCollectionService = lootCollectionService;
            _lootCollection = context.GetGroup(MetaMatcher.AllOf(
                MetaMatcher.LootProgression,
                MetaMatcher.LootTypeId));
        }

        public void Initialize()
        {
            List<LootLevelsProgressionData> items = new(_lootCollection.count);
            
            foreach (MetaEntity loot in _lootCollection)
            {
                var item = new LootLevelsProgressionData(loot.LootTypeId, loot.Level);
                
                items.Add(item);
            }

            _lootCollectionService.InitializeLootProgression(items);
        }
    }
}