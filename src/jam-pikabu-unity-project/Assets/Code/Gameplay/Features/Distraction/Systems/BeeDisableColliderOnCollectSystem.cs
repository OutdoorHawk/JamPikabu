using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.Distraction.Systems
{
    public class BeeDisableColliderOnCollectSystem : ReactiveSystem<GameEntity>
    {
        public BeeDisableColliderOnCollectSystem(GameContext context) : base(context)
        {
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher
                .AllOf(GameMatcher.Bee, GameMatcher.MarkedForPickup)
                .Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isBee && entity.hasBeeBehaviour && entity.isMarkedForPickup;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.isMarkedForPickup = false;
                entity.BeeBehaviour.DisableCollider();
            }
        }
    }
}