using Code.Gameplay.Features.Loot.Behaviours;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Orders.Config;
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

        public LootItemUI CreateLootItem(Transform parent, in IngredientData ingredientData)
        {
            var lootStaticData = _staticData.Get<LootSettingsStaticData>();
            var prefab = lootStaticData.LootItemUI;
            var lootItemUI = _instantiator.InstantiatePrefabForComponent<LootItemUI>(prefab, parent);
            var lootSetup = lootStaticData.GetConfig(ingredientData.TypeId);
            lootItemUI.InitType(lootSetup);
            lootItemUI.InitItem(in ingredientData);
            lootItemUI.Hide();
            return lootItemUI;
        }
    }
}