using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Gameplay.Features.Loot.Behaviours;
using Code.Gameplay.Features.Loot.UIFactory;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Windows;
using Code.Meta.UI.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.Orders.Windows
{
    public class OrderWindow : BaseWindow
    {
        [SerializeField] private Image _orderIcon;
        [SerializeField] private HorizontalLayoutGroup _goodIngredients;
        [SerializeField] private HorizontalLayoutGroup _badIngredients;
        [SerializeField] private PriceInfo _orderReward;
        [SerializeField] private float _startCompleteAnimationDelay = 0.6f;

        private IOrdersService _ordersService;
        private ILootItemUIFactory _lootItemUIFactory;

        private OrderData _currentOrder;

        private readonly List<LootItemUI> _goodItems = new();
        private readonly List<LootItemUI> _badItems = new();
        private readonly List<LootItemUI> _allItems = new();

        public Button ExitButton => CloseButton;

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

        public async UniTask PlayOrderComplete()
        {
            await DelaySeconds(_startCompleteAnimationDelay,destroyCancellationToken);
            await PlayOrderCompleteInternal();
        }

        private void InitOrder()
        {
            _currentOrder = _ordersService.GetCurrentOrder();

            _orderIcon.sprite = _currentOrder.Setup.OrderIcon;

            CreateItems(_currentOrder.Setup.GoodIngredients, _goodItems, _goodIngredients.transform);
            CreateItems(_currentOrder.Setup.BadIngredients, _badItems, _badIngredients.transform);

            _orderReward.SetupPrice(_currentOrder.Setup.Reward);
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
            item.InitPrice(ingredientData.Rating);

            _allItems.Add(item);
            return item;
        }

        private async UniTask PlayOrderCompleteInternal()
        {
            foreach (var lootItem in _allItems)
            {
                await lootItem.AnimateConsume();
            }
        }
    }
}