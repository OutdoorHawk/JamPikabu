using Code.Gameplay.Features.Orders.Service;
using Entitas;

namespace Code.Gameplay.Features.Orders
{
    public class InitOrdersDaySystem : IInitializeSystem
    {
        private readonly IOrdersService _ordersService;

        public InitOrdersDaySystem(GameContext context, IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        public void Initialize()
        {
            _ordersService.InitDay();
        }
    }
}