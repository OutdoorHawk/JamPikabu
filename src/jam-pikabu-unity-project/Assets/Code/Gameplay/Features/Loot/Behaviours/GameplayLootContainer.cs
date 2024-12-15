using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Loot.UIFactory;
using Code.Gameplay.Features.Orders;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Features.Orders.Service;
using Cysharp.Threading.Tasks;
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

        private ILootService _lootService;
        private ILootItemUIFactory _lootItemUIFactory;
        private IOrdersService _ordersService;

        public readonly Dictionary<LootTypeId, LootItemUI> ItemsByLootType = new();

        private readonly List<UniTask> _tasksBuffer = new(16);

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

        public async UniTask AnimateFlyToVat()
        {
            await ProcessAnimation();
        }

        private async UniTask ProcessAnimation()
        {
            const float interval = 0.25f;
            _tasksBuffer.Clear();

            foreach (var lootItemUI in ItemsByLootType.Values)
            {
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
    }
}