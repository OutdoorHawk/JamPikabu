using Entitas;

namespace Code.Gameplay.Features.Orders.Systems
{
    public class ClearCompletedOrdersSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _entities;

        public ClearCompletedOrdersSystem(GameContext context)
        {
            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Order, 
                    GameMatcher.Complete
                ));
        }

        public void Cleanup()
        {
            foreach (var entity in _entities)
            {
                entity.isDestructed = true;
            }
        }
    }
}