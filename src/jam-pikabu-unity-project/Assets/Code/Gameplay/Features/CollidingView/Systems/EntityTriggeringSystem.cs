using System.Collections.Generic;
using Code.Gameplay.Common.Collisions;
using Entitas;

namespace Code.Gameplay.Features.CollidingView.Systems
{
    public class EntityTriggeringSystem : ReactiveSystem<GameEntity>
    {
        private readonly ICollisionRegistry _collisionRegistry;

        public EntityTriggeringSystem(GameContext context, ICollisionRegistry collisionRegistry) : base(context)
        {
            _collisionRegistry = collisionRegistry;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(GameMatcher.TriggerCollider).Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasTriggerCollider;
        }

        protected override void Execute(List<GameEntity> collidingEntities)
        {
            foreach (var entered in collidingEntities)
            {
                int key = entered.TriggerCollider.GetInstanceID();

                if (_collisionRegistry.TryGet(key, out IEntity collidedEntity) == false)
                    continue;

                if (collidedEntity is not GameEntity gameEntity)
                    continue;

                if (collidedEntity.isEnabled == false)
                    continue;

                HandleEntityTrigger(gameEntity, entered);
            }
        }

        private void HandleEntityTrigger(GameEntity collidedEntity, GameEntity entered)
        {
            if (collidedEntity.Id == entered.Id)
                return;

            collidedEntity.ReplaceTriggeredBy(entered.Id);
            collidedEntity.ReplaceTriggerCollider(entered.TriggerCollider);
            entered.ReplaceTriggeredBy(collidedEntity.Id);
        }
    }

}