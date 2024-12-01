using Code.Gameplay.Features.Loot.Behaviours;
using Code.Infrastructure.View.Registrars;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Registrars
{
    public class LootItemUIRegistrar : EntityComponentRegistrar
    {
        [SerializeField] private LootItemUI _lootItemUI;

        public override void RegisterComponents()
        {
            Entity.AddLootItemUI(_lootItemUI);
        }

        public override void UnregisterComponents()
        {
            if (Entity.hasLootItemUI)
                Entity.RemoveLootItemUI();
        }
    }
}