using Code.Gameplay.Features.Orders.Config;

namespace Code.Gameplay.Features.Orders.Service
{
    public interface IOrdersService
    {
        void InitDay();
        OrderData GetCurrentOrder();
        void GoToNextOrder();
    }
}