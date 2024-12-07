using System;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Orders.Config;

namespace Code.Gameplay.Features.Orders.Service
{
    public interface IOrdersService
    {
        event Action OnOrderUpdated;
        int OrdersCompleted { get; }
        int MaxOrders { get; }
        void InitDay(int currentDay);
        GameEntity CreateOrder();
        void AddIngredientTypedData(LootTypeId lootLootTypeId, IngredientData ingredientData);
        IngredientData GetIngredientData(LootTypeId lootTypeId);
        bool TryGetIngredientData(LootTypeId lootTypeId, out IngredientData ingredientData);
        OrderData GetCurrentOrder();
        bool CheckOrdersCompleted();
        void GoToNextOrder();
        void GameOver();
    }
}