using Code.Gameplay.Features.Loot.Behaviours;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.StaticData;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.Loot.UIFactory
{
    public class LootItemUIFactory : ILootItemUIFactory
    {
        private readonly IStaticDataService _staticData;
        private readonly IInstantiator _instantiator;

        public LootItemUIFactory(IStaticDataService staticDataService, IInstantiator instantiator)
        {
            _staticData = staticDataService;
            _instantiator = instantiator;
        }

        public LootItemUI CreateLootItem(Transform parent, LootSetup setup)
        {
            var prefab = _staticData.GetStaticData<LootStaticData>().LootItemUI;
            var lootItemUI = _instantiator.InstantiatePrefabForComponent<LootItemUI>(prefab, parent);
            lootItemUI.Init(setup);
            return lootItemUI;
        }
        
        public LootItemUI CreateLootItem(Transform parent, LootTypeId type)
        {
            var lootStaticData = _staticData.GetStaticData<LootStaticData>();
            var prefab = lootStaticData.LootItemUI;
            var lootItemUI = _instantiator.InstantiatePrefabForComponent<LootItemUI>(prefab, parent);
            var lootSetup = lootStaticData.GetConfig(type);
            lootItemUI.Init(lootSetup);
            return lootItemUI;
        }
    }
}