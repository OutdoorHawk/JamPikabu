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
        public Image VatIcon;

        private readonly List<LootItemUI> _items = new();

        private ILootService _lootService;
        private ILootItemUIFactory _lootUIFactory;

        public List<LootItemUI> Items => _items;

        [Inject]
        private void Construct(ILootService lootService, ILootItemUIFactory lootUIFactory)
        {
            _lootUIFactory = lootUIFactory;
            _lootService = lootService;
        }

        private void Start()
        {
            _lootService.OnLootUpdate += Refresh;
            _lootService.OnLootItemAdded += AddNewItem;
        }

        private void OnDestroy()
        {
            _lootService.OnLootUpdate -= Refresh;
            _lootService.OnLootItemAdded -= AddNewItem;
        }

        private void AddNewItem(LootTypeId type)
        {
            LootItemUI lootItemUI = _lootUIFactory.CreateLootItem(LootGrid.transform, type);
            lootItemUI.CanvasGroup.alpha = 0;
            _items.Add(lootItemUI);
        }

        private void Refresh()
        {
        }
    }
}