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

            CreateItems(_currentOrder.Setup.GoodIngredients, _goodItems);
            CreateItems(_currentOrder.Setup.BadIngredients, _badItems);

        }

        private void CreateItems(List<IngredientData> ingredients, List<LootItemUI> items)
        {
            foreach (IngredientData ingredientData in ingredients)
            {
                LootItemUI item = CreateLootItem(ingredientData);
                items.Add(item);
            }
        }

        private LootItemUI CreateLootItem(in IngredientData ingredientData)
        {
            LootItemUI item = _lootItemUIFactory.CreateLootItem(_goodIngredients.transform, ingredientData.TypeId);
            item.Value.SetupPrice(ingredientData.Rating);
            _allItems.Add(item);
            return item;
        }
    }
}