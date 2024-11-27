using System.Collections.Generic;
using Code.Gameplay.Common.Collisions;
using Entitas;

namespace Code.Gameplay.Features.CollidingView.Systems
{
    public class EntityCollisionSystem : ReactiveSystem<GameEntity>
    {
        private readonly ICollisionRegistry _collisionRegistry;

        public EntityCollisionSystem(GameContext context, ICollisionRegistry collisionRegistry) : base(context)
        {
            _collisionRegistry = collisionRegistry;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Collision).Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasCollision;
        }

        protected override void Execute(List<GameEntity> collidingEntities)
        {
            foreach (var entered in collidingEntities)
            {
                int key = entered.Collision.collider.GetInstanceID();

                if (_collisionRegistry.TryGet(key, out IEntity collidedEntity) == false)
                    continue;

                if (collidedEntity is not GameEntity gameEntity)
                    continue;

                if (collidedEntity.isEnabled == false)
                    continue;

                HandleEntityCollision(gameEntity, entered);
            }
        }

        private void HandleEntityCollision(GameEntity collidedEntity, GameEntity entered)
        {
            if (collidedEntity.id.Value == entered.id.Value)
                return;

            collidedEntity.ReplaceCollidedBy(entered.Id);
            collidedEntity.ReplaceCollision(entered.collision.Value);
            entered.ReplaceCollidedBy(collidedEntity.Id);
        }
    }
    
    
    /*public class EntityCollisionSystem : IExecuteSystem  
    {
    	private readonly IGroup<GameEntity> _entities;
        private readonly ICollisionRegistry _collisionRegistry;
        private readonly List<GameEntity> _buffer = new(32);

        public EntityCollisionSystem(GameContext context, ICollisionRegistry collisionRegistry)
        {
            _collisionRegistry = collisionRegistry;
            _entities = context.GetGroup(GameMatcher
                    .AllOf(
                      GameMatcher.Collision
                        ));
        }
    
    	public void Execute() 
    	{
    		foreach (var entered in _entities.GetEntities(_buffer))
    		{
                int key = entered.Collision.collider.GetInstanceID();

                if (_collisionRegistry.TryGet(key, out IEntity collidedEntity) == false)
                    continue;

                if (collidedEntity is not GameEntity gameEntity)
                    continue;

                if (collidedEntity.isEnabled == false)
                    continue;

                HandleEntityCollision(gameEntity, entered);
    		}
    	}
        
        private void HandleEntityCollision(GameEntity collidedEntity, GameEntity entered)
        {
            if (collidedEntity.id.Value == entered.id.Value)
                return;

            collidedEntity.ReplaceCollidedBy(entered.Id);
            collidedEntity.ReplaceCollision(entered.collision.Value);
            entered.ReplaceCollidedBy(collidedEntity.Id);
        }
    }*/
}