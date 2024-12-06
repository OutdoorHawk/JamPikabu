using Code.Gameplay.Features.Orders.Service;
using Entitas;

namespace Code.Gameplay.Features.Orders.Systems
{
    public class InitDayOrdersSystem : IInitializeSystem
    {
        private readonly IOrdersService _ordersService;
        private readonly IGroup<GameEntity> _roundState;

        public InitDayOrdersSystem(GameContext gameContext, IOrdersService ordersService)
        {
            _ordersService = ordersService;

            _roundState = gameContext.GetGroup(GameMatcher.RoundStateController);
        }

        public void Initialize()
        {
            foreach (GameEntity gameEntity in _roundState)
            {
                _ordersService.InitDay(gameEntity.Day);
            }
        }
    }
}