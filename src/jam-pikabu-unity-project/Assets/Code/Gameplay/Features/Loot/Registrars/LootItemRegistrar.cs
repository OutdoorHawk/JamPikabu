using Code.Gameplay.Features.Loot.Behaviours;
using Code.Infrastructure.View.Registrars;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Registrars
{
    public class LootItemRegistrar : EntityComponentRegistrar
    {
        [SerializeField] private LootItem _lootItem;

        public override void RegisterComponents()
        {
            Entity.AddLootItem(_lootItem);
        }

        public override void UnregisterComponents()
        {
            if (Entity.hasLootItem)
                Entity.RemoveLootItem();
        }
    }
}