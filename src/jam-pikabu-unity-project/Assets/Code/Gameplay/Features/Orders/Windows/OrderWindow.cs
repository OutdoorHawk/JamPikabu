using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Behaviours;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.Loot.Behaviours;
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

        private OrderData _currentOrder;

        private readonly List<LootItemUI> _goodItems = new();
        private readonly List<LootItemUI> _badItems = new();
        private readonly List<LootItemUI> _allItems = new();

        public Button ExitButton => CloseButton;

        [Inject]
        private void Construct(IOrdersService ordersService, ILootItemUIFactory lootItemUIFactory,
            ICurrencyFactory currencyFactory, IStaticDataService staticDataService)
        {
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
                await lootItem.AnimateConsume();

                var parameters = new CurrencyAnimationParameters()
                {
                    Type = CurrencyTypeId.Plus,
                    Count = 5,
                    StartPosition = lootItem.transform.position,
                    EndPosition = _currencyHolder.PlayerPluses.CurrencyIcon.transform.position,
                    StartReplenishCallback = () =>
                    {
                        /*CreateGameEntity.Empty()
                            .With(x => x.isAddCurrencyRequest = true)
                            .With(x => x.AddPlus(loot.Plus), when: loot.hasPlus)
                            .With(x => x.AddMinus(loot.Minus), when: loot.hasMinus)
                            .With(x => x.AddWithdraw(loot.Plus), when: loot.hasPlus)
                            .With(x => x.AddWithdraw(loot.Minus), when: loot.hasMinus)
                            ;*/
                    }
                };

                _currencyFactory.PlayCurrencyAnimation(parameters);
            }

            /*
            foreach (var lootItem in _goodItems)
            {
                await PlayConsume(lootItem, _currencyHolder.PlayerMinuses, CurrencyTypeId.Plus);
            }
            */
            
            foreach (var lootItem in _badItems)
            {
                await PlayConsume(lootItem, _currencyHolder.PlayerMinuses, CurrencyTypeId.Minus);
            }
        }

        private async UniTask PlayConsume(LootItemUI lootItem, PriceInfo price, CurrencyTypeId typeId)
        {
            await lootItem.AnimateConsume();

            var parameters = new CurrencyAnimationParameters()
            {
                Type = typeId,
                Count = 5,
                StartPosition = lootItem.transform.position,
                EndPosition = price.CurrencyIcon.transform.position
            };

            _currencyFactory.PlayCurrencyAnimation(parameters);
        }
    }
}