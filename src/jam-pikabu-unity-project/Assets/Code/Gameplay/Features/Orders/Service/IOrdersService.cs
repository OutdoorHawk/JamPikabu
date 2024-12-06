using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Orders.Config;

namespace Code.Gameplay.Features.Orders.Service
{
    public interface IOrdersService
    {
        void InitDay();
        GameEntity CreateOrder();
        IngredientData GetIngredientData(LootTypeId lootTypeId);
        OrderData GetCurrentOrder();
        void GoToNextOrder();
        void GameOver();
    }
}