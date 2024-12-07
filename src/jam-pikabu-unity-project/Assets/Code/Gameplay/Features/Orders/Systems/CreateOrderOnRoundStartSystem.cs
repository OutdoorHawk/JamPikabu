using System.Collections.Generic;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.Features.Orders.Service;
using Entitas;

namespace Code.Gameplay.Features.Orders.Systems
{
    public class CreateOrderOnRoundStartSystem : ReactiveSystem<GameEntity>
    {
        private readonly IOrdersService _ordersService;
        private readonly IGroup<GameEntity> _loot;

        public CreateOrderOnRoundStartSystem(GameContext context, IOrdersService ordersService) : base(context)
        {
            _ordersService = ordersService;

            _loot = context.GetGroup(GameMatcher.AllOf(
                GameMatcher.Loot));
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(
                GameMatcher.RoundStateController,
                GameMatcher.RoundInProcess).Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isRoundStateController && entity.isRoundInProcess;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            _ordersService.CreateOrder();
        }
    }
}