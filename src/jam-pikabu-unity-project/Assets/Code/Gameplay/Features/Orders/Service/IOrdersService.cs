using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Orders.Config;

namespace Code.Gameplay.Features.Orders.Service
{
    public interface IOrdersService
    {
        void InitDay(int currentDay);
        GameEntity CreateOrder();
        void AddIngredientTypedData(LootTypeId lootLootTypeId, IngredientData ingredientData);
        IngredientData GetIngredientData(LootTypeId lootTypeId);
        OrderData GetCurrentOrder();
        bool OrdersCompleted();
        void GoToNextOrder();
        void GameOver();
    }
}