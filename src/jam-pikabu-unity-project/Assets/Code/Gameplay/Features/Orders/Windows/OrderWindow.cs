using System.Collections.Generic;
using System.Linq;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Common.Extensions.Animations;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Behaviours;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.GameOver.Service;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Behaviours;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Loot.UIFactory;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Features.RoundState.Service;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Code.Meta.UI.Common;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.Orders.Windows
{
    public class OrderWindow : BaseWindow
    {
        [SerializeField] private Image _orderIcon;
        [SerializeField] private RectTransform _goodIngredients;
        [SerializeField] private RectTransform _badIngredients;
        [SerializeField] private PriceInfo _orderReward;
        [SerializeField] private CurrencyHolder _currencyHolder;
        [SerializeField] private Animator _closeAnimator;
        [SerializeField] private float _startCompleteAnimationDelay = 0.6f;
        [SerializeField] private GameObject _bossContent;
        [SerializeField] private Animator _bossIconAnimator;
        [SerializeField] private TMP_Text _atLeastGoodText;

        private IOrdersService _ordersService;
        private ILootItemUIFactory _lootItemUIFactory;
        private IStaticDataService _staticDataService;
        private ICurrencyFactory _currencyFactory;
        private ISoundService _soundService;
        private ILootService _lootService;
        private IWindowService _windowService;
        private IRoundStateService _roundStateService;

        private OrderData _currentOrder;

        private readonly List<LootItemUI> _goodItems = new();
        private readonly List<LootItemUI> _badItems = new();
        private readonly List<LootItemUI> _allItems = new();
        private IGameOverService _gameOverService;

        public Button ExitButton => CloseButton;

        [Inject]
        private void WConstruct(IOrdersService ordersService, ILootItemUIFactory lootItemUIFactory,
            ICurrencyFactory currencyFactory, IStaticDataService staticDataService, ILootService lootService, ISoundService soundService,
            IRoundStateService roundStateService,
            IWindowService windowService, IGameOverService gameOverService)
        {
            _gameOverService = gameOverService;
            _roundStateService = roundStateService;
            _windowService = windowService;
            _soundService = soundService;
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
            InitBoss();
        }

        private void InitBoss()
        {
            if (_currentOrder.Setup.GoodMinimum > 0)
            {
                _bossContent.SetActive(true);
                _atLeastGoodText.text += $" {_currentOrder.Setup.GoodMinimum}";
            }
        }

        protected override void CloseWindowInternal()
        {
            base.CloseWindowInternal();
            _closeAnimator.SetTrigger(AnimationParameter.Hide.AsHash());
        }

        public async UniTask PlayOrderComplete(UniTaskCompletionSource completion, bool orderSusscesful)
        {
            await DelaySeconds(_startCompleteAnimationDelay, destroyCancellationToken);

            await PlayLootAnimationInternal();
            
            if (orderSusscesful)
            {
                if (_roundStateService.GetDayData().IsBoss == false)
                    await PlayGoldAnimation();
                else
                    await PlayBossDoneAnimation();

                return;
            }

            await PlayBossDefeatAnimation(completion);
        }

        private void InitOrder()
        {
            _currentOrder = _ordersService.GetCurrentOrder();

            _orderIcon.sprite = _currentOrder.Setup.OrderIcon;

            CreateItems(_ordersService.OrderIngredients.good, _goodItems, _goodIngredients.transform);
            CreateItems(_ordersService.OrderIngredients.bad, _badItems, _badIngredients.transform);

            _orderReward.SetupPrice(_currentOrder.Setup.Reward);

            _ordersService.SetOrderWindowSeen();
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

        private async UniTask PlayLootAnimationInternal()
        {
            foreach (var lootItem in _goodItems)
            {
                await PlayConsume(lootItem, _currencyHolder.PlayerPluses, CurrencyTypeId.Plus, isGood: true);
            }

            foreach (var lootItem in _badItems)
            {
                await PlayConsume(lootItem, _currencyHolder.PlayerMinuses, CurrencyTypeId.Minus, false);
            }
        }

        private async UniTask PlayBossDefeatAnimation(UniTaskCompletionSource completion)
        {
            Close();
            completion.TrySetResult();
        }

        private async UniTask PlayConsume(LootItemUI lootItem, PriceInfo price, CurrencyTypeId typeId, bool isGood)
        {
            IngredientData ingredientData = _ordersService.GetIngredientData(lootItem.Type);
            IEnumerable<LootTypeId> collected = _lootService.CollectedLootItems.Where(item => item == ingredientData.TypeId);
            int count = collected.Count();

            if (count == 0)
                return;

            var parameters = new CurrencyAnimationParameters
            {
                Type = typeId,
                Count = count,
                StartPosition = lootItem.transform.position,
                EndPosition = price.CurrencyIcon.transform.position,
                StartReplenishSound = SoundTypeId.Soft_Currency_Collect,
                StartReplenishCallback = () => RemoveWithdraw(price, ingredientData, count * ingredientData.Rating.Amount)
            };

            _soundService.PlayOneShotSound(SoundTypeId.PlusesAdded);
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

        private async UniTask PlayBossDoneAnimation()
        {
            _soundService.PlayOneShotSound(SoundTypeId.Level_Win);
            await _bossIconAnimator.WaitForAnimationCompleteAsync(AnimationParameter.Win.AsHash());
            _gameOverService.GameWin();
        }

        private async UniTask PlayGoldAnimation()
        {
            await DelaySeconds(1f, destroyCancellationToken);

            var parameters = new CurrencyAnimationParameters
            {
                Type = _currentOrder.Setup.Reward.CurrencyType,
                Count = _currentOrder.Setup.Reward.Amount,
                StartReplenishSound = SoundTypeId.Gold_Currency_Collect,
                StartPosition = _orderReward.CurrencyIcon.transform.position,
                EndPosition = _currencyHolder.PlayerCurrentGold.CurrencyIcon.transform.position,
                StartReplenishCallback = () => RemoveWithdraw(_currentOrder.Setup.Reward.Amount)
            };

            _soundService.PlaySound(SoundTypeId.Order_Completed);
            _currencyFactory.PlayCurrencyAnimation(parameters);
        }

        private void RemoveWithdraw(int rewardAmount)
        {
            CreateGameEntity
                .Empty()
                .With(x => x.isAddCurrencyRequest = true)
                .AddGold(0)
                .AddWithdraw(-rewardAmount)
                ;
        }
    }
}