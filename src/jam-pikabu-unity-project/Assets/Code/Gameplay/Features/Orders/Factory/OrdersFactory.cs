using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Orders.Config;

namespace Code.Gameplay.Features.Orders.Factory
{
    public class OrdersFactory : IOrdersFactory
    {
        public GameEntity CreateOrder(OrderData order)
        {
            return CreateGameEntity
                    .Empty()
                    .With(x => x.isOrder = true)
                    .With(x => x.isBossOrder = true, when: order.Setup.IsBoss)
                    .AddOrderData(order)
                ;
        }
    }
}