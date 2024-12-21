using Code.Gameplay.Features.Currency.Behaviours;
using Code.Gameplay.Features.Loot.Behaviours;
using Code.Gameplay.Features.Orders.Behaviours;
using Code.Gameplay.Windows;
using UnityEngine;

namespace Code.Gameplay.Features.HUD
{
    public class PlayerHUDWindow : BaseWindow
    {
        public OrderViewBehaviour OrderViewBehaviour;
        public CurrencyHolder CurrencyHolder;
        public GameplayLootContainer LootContainer;
        public Transform LootPoint;
        public Transform HookPoint;
    }
}