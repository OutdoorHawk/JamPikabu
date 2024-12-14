using System.Collections.Generic;
using System.Linq;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Loot.UIFactory;
using Code.Gameplay.Features.Orders;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Features.Orders.Service;
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
        private readonly List<LootItemUI> _goodItems = new();
        private readonly List<LootItemUI> _badItems = new();

        private ILootService _lootService;
        private ILootItemUIFactory _lootItemUIFactory;
        private IOrdersService _ordersService;

        public List<LootItemUI> Items => _items;
        
        public Dictionary<LootTypeId, LootItemUI> ItemsByLootType = new();

        [Inject]
        private void Construct
        (
            ILootService lootService,
            ILootItemUIFactory lootUIFactory,
            IOrdersService ordersService
        )
        {
            _ordersService = ordersService;
            _lootItemUIFactory = lootUIFactory;
            _lootService = lootService;
        }

        private void Start()
        {
            _ordersService.OnOrderUpdated += RefreshCurrentOrder;
        }

        private void OnDestroy()
        {
            _ordersService.OnOrderUpdated -= RefreshCurrentOrder;
        }

        private void RefreshCurrentOrder()
        {
            ItemsByLootType.Clear();
            
            (List<IngredientData> good, List<IngredientData> bad) = _ordersService.OrderIngredients;

            foreach (IngredientData data in good) 
                CreateLootItem(data, LootGrid.transform);
            
            foreach (IngredientData data in bad) 
                CreateLootItem(data, LootGrid.transform);
        }

        private void CreateLootItem(in IngredientData ingredientData, Transform parent)
        {
            LootItemUI item = _lootItemUIFactory.CreateLootItem(parent, ingredientData);

            switch (ingredientData.IngredientType)
            {
                case IngredientTypeId.Good:
                    CreateGoodIngredient(ingredientData, item);
                    break;
                case IngredientTypeId.Bad:
                    CreateBadIngredient(ingredientData, item);
                    break;
            }
            
            ItemsByLootType.Add(ingredientData.TypeId, item);
        }

        private void CreateGoodIngredient(in IngredientData ingredientData, LootItemUI item)
        {
            item.InitRatingFactor(new CostSetup
            {
                CurrencyType = CurrencyTypeId.Plus,
                Amount = ingredientData.RatingFactor
            });
            
            _goodItems.Add(item);
        }

        private void CreateBadIngredient(in IngredientData ingredientData, LootItemUI item)
        {
            item.InitRatingFactor(new CostSetup
            {
                CurrencyType = CurrencyTypeId.Minus,
                Amount = ingredientData.RatingFactor
            });

            _badItems.Add(item);
        }
    }
}