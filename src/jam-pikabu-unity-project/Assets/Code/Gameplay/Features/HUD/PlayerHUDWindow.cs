﻿using Code.Gameplay.Features.Consumables.Behaviours;
using Code.Gameplay.Features.Currency.Behaviours;
using Code.Gameplay.Features.Loot.Behaviours;
using Code.Gameplay.Features.Orders.Behaviours;
using Code.Gameplay.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.Features.HUD
{
    public class PlayerHUDWindow : BaseWindow
    {
        public OrderViewBehaviour OrderViewBehaviour;
        public CurrencyHolder CurrencyHolder;
        public GameplayLootContainer LootContainer;
        public ConsumablesBoostersHolder ConsumablesHolder;
        public Transform LootPoint;
        public Transform BonfirePoint;
        public Transform HookPoint;
        public Button TimerButton;
    }
}