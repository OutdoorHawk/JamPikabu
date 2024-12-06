using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Features.RoundState.Service;
using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class ProcessNextOrderRequest : IExecuteSystem
    {
        private readonly IRoundStateService _roundStateService;
        private readonly IOrdersService _ordersService;
        private readonly IGroup<GameEntity> _entities;
        private readonly IGroup<GameEntity> _roundStateController;

        public ProcessNextOrderRequest(GameContext context, IRoundStateService roundStateService, IOrdersService ordersService)
        {
            _roundStateService = roundStateService;
            _ordersService = ordersService;

            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.NextOrderRequest
                ));

            _roundStateController = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.RoundStateController
                ));
        }

        public void Execute()
        {
            foreach (var entity in _entities)
            foreach (var _ in _roundStateController)
            {
                entity.isDestructed = true;
                
                _ordersService.GoToNextOrder();

                if (_ordersService.OrdersCompleted())
                {
                    _roundStateService.DayComplete();
                }
            }
        }
    }
}