using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Orders.Config;
using Entitas;

namespace Code.Gameplay.Features.Orders.Systems
{
    public class CompleteOrderOnRoundOverSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _orders;
        private readonly IGroup<GameEntity> _roundStateControllers;

        public CompleteOrderOnRoundOverSystem(GameContext context)
        {
            _roundStateControllers = context.GetGroup(
                GameMatcher.AllOf(GameMatcher.RoundStateController, 
                    GameMatcher.RoundComplete));
            
            _orders = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Order,
                    GameMatcher.OrderData
                ));
        }

        public void Execute()
        {
            foreach (var _ in _roundStateControllers)
            foreach (var entity in _orders)
            {
                OrderSetup orderDataSetup = entity.OrderData.Setup;
                
                CreateGameEntity
                    .Empty()
                    .With(x => x.isAddCurrencyRequest = true)
                    .AddGold(orderDataSetup.Reward.Amount)
                    ;
                
                //entity.isCom
            }
        }
    }
}