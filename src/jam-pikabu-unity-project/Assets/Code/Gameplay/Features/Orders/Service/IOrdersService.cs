using Code.Gameplay.Features.Orders.Config;

namespace Code.Gameplay.Features.Orders.Service
{
    public interface IOrdersService
    {
        void InitDay();
        GameEntity CreateOrder();
        OrderData GetCurrentOrder();
        void GoToNextOrder();
        void GameOver();
    }
}