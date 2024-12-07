using Code.Gameplay.Features.Orders.Service;
using Entitas;

namespace Code.Gameplay.Features.Orders.Systems
{
    public class InitOrderSystem : IInitializeSystem
    {
        private readonly IOrdersService _ordersService;

        public InitOrderSystem(GameContext context, IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        public void Initialize()
        {
            _ordersService.CreateOrder();
        }
    }
}