using System.Collections.Generic;
using System.Linq;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Behaviours;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Behaviours;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Loot.UIFactory;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.StaticData;
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
        [SerializeField] private CurrencyHolder _currencyHolder;
        [SerializeField] private float _startCompleteAnimationDelay = 0.6f;

        private IOrdersService _ordersService;
        private ILootItemUIFactory _lootItemUIFactory;
        private IStaticDataService _staticDataService;
        private ICurrencyFactory _currencyFactory;
        private ILootService _lootService;

        private OrderData _currentOrder;

        private readonly List<LootItemUI> _goodItems = new();
        private readonly List<LootItemUI> _badItems = new();
        private readonly List<LootItemUI> _allItems = new();

        public Button ExitButton => CloseButton;

        [Inject]
        private void Construct(IOrdersService ordersService, ILootItemUIFactory lootItemUIFactory,
            ICurrencyFactory currencyFactory, IStaticDataService staticDataService, ILootService lootService)
        {
            _lootService = lootService;
            _staticDataService = staticDataService;
            _currencyFactory = currencyFactory;
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
            await DelaySeconds(_startCompleteAnimationDelay, destroyCancellationToken);
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
            foreach (var lootItem in _goodItems)
            {
                await PlayConsume(lootItem, _currencyHolder.PlayerPluses, CurrencyTypeId.Plus);
            }

            foreach (var lootItem in _badItems)
            {
                await PlayConsume(lootItem, _currencyHolder.PlayerMinuses, CurrencyTypeId.Minus);
            }
        }

        private async UniTask PlayConsume(LootItemUI lootItem, PriceInfo price, CurrencyTypeId typeId)
        {
            IngredientData ingredientData = _ordersService.GetIngredientData(lootItem.Type);
            IEnumerable<LootTypeId> collected = _lootService.CollectedLootItems.Where(item => item == ingredientData.TypeId);
            int count = ingredientData.Rating.Amount * collected.Count();

            var parameters = new CurrencyAnimationParameters
            {
                Type = typeId,
                Count = count,
                StartPosition = lootItem.transform.position,
                EndPosition = price.CurrencyIcon.transform.position,
                StartReplenishCallback = () => RemoveWithdraw(price,ingredientData, count)
            };
            
            await lootItem.AnimateConsume();
            
            _currencyFactory.PlayCurrencyAnimation(parameters);
        }

        private static void RemoveWithdraw(PriceInfo price, IngredientData ingredientData, int count)
        {
            if (price != null) 
                price.PlayReplenish();
            
            CreateGameEntity.Empty()
                .With(x => x.isAddCurrencyRequest = true)
                .With(x => x.AddPlus(0), when: ingredientData.Rating.CurrencyType == CurrencyTypeId.Plus)
                .With(x => x.AddMinus(0), when: ingredientData.Rating.CurrencyType == CurrencyTypeId.Minus)
                .With(x => x.AddWithdraw(-count))
                ;
        }
    }
}