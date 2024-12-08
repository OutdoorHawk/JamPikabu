using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Orders.Config;

namespace Code.Gameplay.Features.Orders.Service
{
    public interface IOrdersService
    {
        event Action OnOrderUpdated;
        int OrdersCompleted { get; }
        int MaxOrders { get; }
        bool OrderWindowSeen { get; }
        (List<IngredientData> good, List<IngredientData> bad) OrderIngredients { get; }
        void InitDay(int currentDay);
        GameEntity CreateOrder();
        void SetOrderWindowSeen();
        IngredientData GetIngredientData(LootTypeId lootTypeId);
        bool TryGetIngredientData(LootTypeId lootTypeId, out IngredientData ingredientData);
        OrderData GetCurrentOrder();
        bool CheckAllOrdersCompleted();
        void GoToNextOrder();
        void GameOver();
        bool OrderPassesConditions();
    }
}