using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Orders.Config;

namespace Code.Gameplay.Features.Orders.Service
{
    public interface IOrdersService
    {
        event Action OnOrderUpdated;
        int OrdersCompleted { get; }
        int MaxOrders { get; }
        (List<IngredientData> good, List<IngredientData> bad) OrderIngredients { get; }
        OrdersStaticData OrdersData { get; }
        void InitDayBegin();
        GameEntity CreateOrder();
        bool TryGetIngredientData(LootTypeId lootTypeId, out IngredientData ingredientData);
        OrderData GetCurrentOrder();
        bool CheckAllOrdersCompleted();
        float GetOrderProgress();
        float GetGoldPenaltyFactor();
        int GetRatingPenalty();
        void GoToNextOrder();
        void GameOver();
        bool OrderPassesConditions();
        CostSetup GetRewardForOrder();
        bool TryGetBonusRating(out int bonusRatingAmount);
        bool CanApplyOrderCompletedFactor();
        bool CanApplyPerfectOrderFactor();
    }
}