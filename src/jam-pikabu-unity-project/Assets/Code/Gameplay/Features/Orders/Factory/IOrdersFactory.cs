using Code.Gameplay.Features.Orders.Config;

namespace Code.Gameplay.Features.Orders.Factory
{
    public interface IOrdersFactory
    {
        GameEntity CreateOrder(OrderData order);
    }
}