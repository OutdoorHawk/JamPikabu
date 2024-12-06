using System.Collections.Generic;
using Code.Gameplay.Features.Loot.Behaviours;
using Code.Gameplay.Features.Loot.UIFactory;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Features.Orders.Windows
{
    public class OrderWindow : BaseWindow
    {
        [SerializeField] private Image _orderIcon;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _skipButton;
        [SerializeField] private HorizontalLayoutGroup _goodIngredients;
        [SerializeField] private HorizontalLayoutGroup _badIngredients;
        
        private IOrdersService _ordersService;
        private ILootItemUIFactory _lootItemUIFactory;
        
        private OrderData _currentOrder;
        
        private readonly List<LootItemUI> _goodItems = new();
        private readonly List<LootItemUI> _badItems = new();
        private readonly List<LootItemUI> _allItems = new();

        [Inject]
        private void Construct(IOrdersService ordersService, ILootItemUIFactory lootItemUIFactory)
        {
            _lootItemUIFactory = lootItemUIFactory;
            _ordersService = ordersService;
        }

        protected override void Initialize()
        {
            base.Initialize();
            InitOrder();
        }

        private void InitOrder()
        {
            _currentOrder = _ordersService.GetCurrentOrder();

            CreateItems(_currentOrder.Setup.GoodIngredients, _goodItems, _goodIngredients.transform);
            CreateItems(_currentOrder.Setup.BadIngredients, _badItems, _badIngredients.transform);

        }

        private void CreateItems(List<IngredientData> ingredients, List<LootItemUI> items, Transform parent)
        {
            foreach (IngredientData ingredientData in ingredients)
            {
                LootItemUI item = CreateLootItem(ingredientData, parent);
                items.Add(item);
            }
        }

        private LootItemUI CreateLootItem(in IngredientData ingredientData, Transform parent)
        {
            LootItemUI item = _lootItemUIFactory.CreateLootItem(parent, ingredientData.TypeId);
            item.Value.SetupPrice(ingredientData.Rating);
            _allItems.Add(item);
            return item;
        }
    }
}