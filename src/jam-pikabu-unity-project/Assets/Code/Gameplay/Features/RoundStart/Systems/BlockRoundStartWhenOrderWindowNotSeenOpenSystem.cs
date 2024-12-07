using System.Collections.Generic;
using Code.Gameplay.Features.Orders.Service;
using Entitas;

namespace Code.Gameplay.Features.RoundStart.Systems
{
    public class BlockRoundStartWhenOrderWindowNotSeenOpenSystem : IExecuteSystem
    {
        private readonly IOrdersService _ordersService;
        private readonly IGroup<GameEntity> _roundController;

        private readonly List<GameEntity> _buffer = new();

        public BlockRoundStartWhenOrderWindowNotSeenOpenSystem(GameContext gameContext, IOrdersService ordersService)
        {
            _ordersService = ordersService;
            _roundController = gameContext.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundStartAvailable
                ));
        }

        public void Execute()
        {
            foreach (var controller in _roundController.GetEntities(_buffer))
            {
                if (_ordersService.OrderWindowSeen)
                    continue;

                controller.isRoundStartAvailable = false;
            }
        }
    }
}