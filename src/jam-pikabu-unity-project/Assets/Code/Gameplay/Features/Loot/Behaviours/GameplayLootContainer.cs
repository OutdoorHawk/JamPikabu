using System.Collections.Generic;
using System.Linq;
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
using Code.Gameplay.Sound;
using Code.Gameplay.Windows.Service;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.Loot.Behaviours
{
    public class GameplayLootContainer : MonoBehaviour
    {
        public GridLayoutGroup LootGrid;
        public Image VatIcon;
        public float StartFlyToVatAnimationDelay = 0.5f;

        private ILootService _lootService;
        private ILootItemUIFactory _lootItemUIFactory;
        private IOrdersService _ordersService;
        private IWindowService _windowService;
        private ICurrencyFactory _currencyFactory;

        public readonly Dictionary<LootTypeId, LootItemUI> ItemsByLootType = new();

        private readonly List<UniTask> _tasksBuffer = new(16);

        [Inject]
        private void Construct
        (
            ILootService lootService,
            ILootItemUIFactory lootUIFactory,
            IOrdersService ordersService,
            IWindowService windowService,
            ICurrencyFactory currencyFactory
        )
        {
            _currencyFactory = currencyFactory;
            _windowService = windowService;
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

        public async UniTask AnimateFlyToVat(IGroup<GameEntity> consumedLoot)
        {
            await ProcessAnimation(consumedLoot);
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
            await DelaySeconds(0.85f, destroyCancellationToken);

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

        private async UniTask ProcessAnimation(IGroup<GameEntity> consumedLoot)
        {
            const float interval = 0.25f;
            _tasksBuffer.Clear();

            await DelaySeconds(StartFlyToVatAnimationDelay, destroyCancellationToken);
            
            foreach (var lootItemUI in ItemsByLootType.Values)
            {
                PlayCurrencyAnimation(lootItemUI, consumedLoot);
                await DelaySeconds(interval, lootItemUI.destroyCancellationToken);

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

            await UniTask.WhenAll(_tasksBuffer);
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

            int countRating = consumed.Sum(loot => loot.Rating * data.RatingFactor);

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
                EndPosition = progressBar.ProgressText.transform.position,
                StartReplenishSound = SoundTypeId.PlusesAdded,
                StartReplenishCallback = () => _currencyFactory.CreateAddCurrencyRequest(data.RatingType, 0, -countRating)
            };

            _currencyFactory.PlayCurrencyAnimation(parameters);
        }
    }
}