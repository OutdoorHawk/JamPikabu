using Entitas;

namespace Code.Gameplay.Features.CollidingView.Systems
{
    public class CleanupCollidedSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _triggered;

        public CleanupCollidedSystem(Contexts contexts)
        {
            _triggered = contexts.game.GetGroup(GameMatcher
                .AnyOf(
                    GameMatcher.CollidedBy, 
                    GameMatcher.Collision));
        }

        public void Cleanup()
        {
            foreach (GameEntity entity in _triggered.GetEntities()) 
                ResetCollided(entity);
        }

        private void ResetCollided(GameEntity entity)
        {
            if (entity.hasCollision)
                entity.RemoveCollision();

            if (entity.hasCollidedBy)
                entity.RemoveCollidedBy();
        }
    }
}