using Code.Gameplay.Features.Loot.Behaviours;
using Code.Gameplay.Features.Orders.Config;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.UIFactory
{
    public interface ILootItemUIFactory
    {
        LootItemUI CreateLootItem(Transform parent, in IngredientData ingredientData);
    }
}