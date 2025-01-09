using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Behaviours;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Loot.UIFactory;
using Code.Gameplay.Features.Orders;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Features.TextNotification;
using Code.Gameplay.Features.TextNotification.Service;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.Localization;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static System.Threading.CancellationTokenSource;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.Loot.Behaviours
{
    public class GameplayLootContainer : MonoBehaviour
    {
        public GridLayoutGroup LootGrid;
        public RectTransform VatIcon;
        public RectTransform SpecialTextPosition;
        public float StartFlyToVatAnimationDelay = 0.5f;

        private IGameplayLootService _gameplayLootService;
        private ILootItemUIFactory _lootItemUIFactory;
        private IOrdersService _ordersService;
        private IWindowService _windowService;
        private ICurrencyFactory _currencyFactory;
        private ISoundService _soundService;
        private INotificationTextService _notificationText;
        private ILocalizationService _localizationService;

        public readonly Dictionary<LootTypeId, LootItemUI> ItemsByLootType = new();

        private readonly List<UniTask> _tasksBuffer = new(16);

        private CancellationTokenSource _refreshSource = new();

        [Inject]
        private void Construct
        (
            IGameplayLootService gameplayLootService,
            ILootItemUIFactory lootUIFactory,
            IOrdersService ordersService,
            IWindowService windowService,
            ICurrencyFactory currencyFactory,
            ISoundService soundService,
            INotificationTextService notificationText,
            ILocalizationService localizationService
        )
        {
            _localizationService = localizationService;
            _notificationText = notificationText;
            _soundService = soundService;
            _currencyFactory = currencyFactory;
            _windowService = windowService;
            _ordersService = ordersService;
            _lootItemUIFactory = lootUIFactory;
            _gameplayLootService = gameplayLootService;
        }

        private void Start()
        {
            _ordersService.OnOrderUpdated += RefreshCurrentOrder;
        }

        private void OnDestroy()
        {
            _ordersService.OnOrderUpdated -= RefreshCurrentOrder;
        }

        public async UniTask AnimateFlyToVat(IGroup<GameEntity> consumedLoot)
        {
            await ProcessLootAnimation(consumedLoot);
            await PlayBonusRatingAnimationAsync();
            await DelaySeconds(0.4f, _refreshSource.Token);
            //await UniTask.WhenAll(_tasksBuffer);
        }

        private void RefreshCurrentOrder()
        {
            ClearList();

            (List<IngredientData> good, List<IngredientData> bad) = _ordersService.OrderIngredients;

            foreach (IngredientData data in good)
                CreateLootItem(data, LootGrid.transform);

            foreach (IngredientData data in bad)
                CreateLootItem(data, LootGrid.transform);

            ShowAsync().Forget();
        }

        private async UniTaskVoid ShowAsync()
        {
            ResetToken();

            await DelaySeconds(0.85f, _refreshSource.Token);

            foreach (LootItemUI lootItemUI in ItemsByLootType.Values)
                lootItemUI.Show();
        }

        private void ClearList()
        {
            foreach (LootItemUI lootItemUI in ItemsByLootType.Values)
                Destroy(lootItemUI.gameObject);

            ItemsByLootType.Clear();
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
        }

        private void CreateBadIngredient(in IngredientData ingredientData, LootItemUI item)
        {
            item.InitRatingFactor(new CostSetup
            {
                CurrencyType = CurrencyTypeId.Minus,
                Amount = ingredientData.RatingFactor
            });
        }

        private async UniTask ProcessLootAnimation(IGroup<GameEntity> consumedLoot)
        {
            const float interval = 0.25f;
            _tasksBuffer.Clear();

            ResetToken();

            await DelaySeconds(StartFlyToVatAnimationDelay, _refreshSource.Token);

            foreach (var lootItemUI in ItemsByLootType.Values)
            {
                PlayCurrencyAnimation(lootItemUI, consumedLoot);
                await DelaySeconds(interval, _refreshSource.Token);

                if (lootItemUI.CollectedAtLeastOne)
                {
                    UniTask task = lootItemUI.AnimateFlyToVat(VatIcon.transform);
                    _tasksBuffer.Add(task);
                }
                else
                {
                    UniTask task = lootItemUI.AnimateConsume();
                    _tasksBuffer.Add(task);
                }
            }
        }

        private void PlayCurrencyAnimation(LootItemUI lootItemUI, IGroup<GameEntity> consumedLoot)
        {
            LootTypeId type = lootItemUI.Type;

            IEnumerable<GameEntity> consumed = consumedLoot
                .GetEntities()
                .Where(x => x.LootTypeId == type);

            if (_ordersService.TryGetIngredientData(type, out IngredientData data) == false)
                return;

            _windowService.TryGetWindow(out PlayerHUDWindow window);
            var progressBar = window.GetComponentInChildren<RatingProgressBar>();

            int countRating = data.IngredientType switch
            {
                IngredientTypeId.Good => consumed.Sum(loot => loot.Rating * data.RatingFactor),
                IngredientTypeId.Bad => consumed.Sum(_ => _ordersService.GetRatingPenalty() * data.RatingFactor),
                _ => 0
            };

            if (countRating == 0)
                return;

            var parameters = new CurrencyAnimationParameters
            {
                Type = data.RatingType,
                Count = countRating,
                TextPrefix = data.IngredientType == IngredientTypeId.Good
                    ? "+"
                    : "-",
                StartPosition = lootItemUI.transform.position,
                EndPosition = progressBar.RatingFlyPos.transform.position,
                StartReplenishCallback = () => _currencyFactory.CreateAddCurrencyRequest(data.RatingType, 0, -countRating)
            };

            _soundService.PlaySound(SoundTypeId.Soft_Currency_Collect);
            _currencyFactory.PlayCurrencyAnimation(parameters);
        }

        private async UniTask PlayBonusRatingAnimationAsync()
        {
            if (_ordersService.TryGetBonusRating(out int ratingAmount) == false)
                return;
            
            await DelaySeconds(0.25f, _refreshSource.Token);

            OrdersStaticData ordersData = _ordersService.OrdersData;

            _windowService.TryGetWindow(out PlayerHUDWindow window);
            var progressBar = window.GetComponentInChildren<RatingProgressBar>();

            string notificationText = "";

            if (_ordersService.CanApplyPerfectOrderFactor())
            {
                notificationText = _localizationService["GAMEPLAY/PERFECT_ORDER", (ordersData.PerfectOrderRatingBonusFactor * 100).ToString("##")];
            }
            else if (_ordersService.CanApplyOrderCompletedFactor())
            {
                notificationText = _localizationService["GAMEPLAY/COMPLETED_ORDER", (ordersData.OrderCompletedRatingBonusFactor * 100).ToString("##")];
            }
            
            var parameters = new CurrencyAnimationParameters
            {
                Type = CurrencyTypeId.Plus,
                Count = ratingAmount,
                OverrideText = true,
                TextPrefix = string.Empty,
                StartPosition = SpecialTextPosition.position,
                EndPosition = progressBar.RatingFlyPos.transform.position,
                StartReplenishCallback = () => _currencyFactory.CreateAddCurrencyRequest(CurrencyTypeId.Plus, 0, -ratingAmount)
            };
            
            var notificationTextParameters = new NotificationTextParameters()
            {
                StartPosition = SpecialTextPosition.position,
                Text = notificationText
            };

            _soundService.PlaySound(SoundTypeId.Soft_Currency_Collect);
            _currencyFactory.PlayCurrencyAnimation(parameters);
            _notificationText.ShowNotificationText(notificationTextParameters);
        }

        private void ResetToken()
        {
            _refreshSource?.Cancel();
            _refreshSource = CreateLinkedTokenSource(destroyCancellationToken);
        }
    }
}