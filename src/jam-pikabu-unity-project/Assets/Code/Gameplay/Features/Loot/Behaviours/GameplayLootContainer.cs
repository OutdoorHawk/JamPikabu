using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Features.Loot.Behaviours
{
    public class GameplayLootContainer : MonoBehaviour
    {
        public GridLayoutGroup LootGrid;

        private readonly List<LootItemUI> _items = new();
        
        private ILootUIService _lootUIService;

        [Inject]
        private void Construct(ILootUIService lootUIService)
        {
            _lootUIService = lootUIService;
        }

        private void Start()
        {
            _lootUIService.OnLootUpdate += Refresh;
        }

        private void OnDestroy()
        {
            _lootUIService.OnLootUpdate -= Refresh;
        }

        private void Refresh()
        {
            
        }
    }
}