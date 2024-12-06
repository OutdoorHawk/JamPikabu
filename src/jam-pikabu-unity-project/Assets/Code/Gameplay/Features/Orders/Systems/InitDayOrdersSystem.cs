using Code.Gameplay.Features.Orders.Service;
using Entitas;

namespace Code.Gameplay.Features.Orders.Systems
{
    public class InitDayOrdersSystem : IInitializeSystem
    {
        private readonly IOrdersService _ordersService;

        public InitDayOrdersSystem(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        public void Initialize()
        {
            _ordersService.InitDay();
        }
    }
}