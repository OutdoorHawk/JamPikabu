using System.Collections.Generic;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Loot.UIFactory;
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
        private ILootItemUIFactory _lootUIFactory;

        public List<LootItemUI> Items => _items;

        [Inject]
        private void Construct(ILootUIService lootUIService, ILootItemUIFactory lootUIFactory)
        {
            _lootUIFactory = lootUIFactory;
            _lootUIService = lootUIService;
        }

        private void Start()
        {
            _lootUIService.OnLootUpdate += Refresh;
            _lootUIService.OnLootItemAdded += AddNewItem;
        }

        private void OnDestroy()
        {
            _lootUIService.OnLootUpdate -= Refresh;
            _lootUIService.OnLootItemAdded -= AddNewItem;
        }

        private void AddNewItem(LootTypeId type)
        {
            LootItemUI lootItemUI = _lootUIFactory.CreateLootItem(LootGrid.transform, type);
            _items.Add(lootItemUI);
        }

        private void Refresh()
        {
        }
    }
}