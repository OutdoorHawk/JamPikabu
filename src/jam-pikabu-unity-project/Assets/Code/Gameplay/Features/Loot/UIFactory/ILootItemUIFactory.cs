using Code.Gameplay.Features.Loot.Behaviours;
using Code.Gameplay.Features.Loot.Configs;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.UIFactory
{
    public interface ILootItemUIFactory
    {
        LootItemUI CreateLootItem(Transform parent, LootSetup setup);
        LootItemUI CreateLootItem(Transform parent, LootTypeId type);
    }
}