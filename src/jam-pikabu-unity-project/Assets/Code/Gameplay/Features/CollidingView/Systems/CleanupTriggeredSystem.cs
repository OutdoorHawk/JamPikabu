using Entitas;

namespace Code.Gameplay.Features.CollidingView.Systems
{
    public class CleanupTriggeredSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _triggered;

        public CleanupTriggeredSystem(Contexts contexts)
        {
            _triggered = contexts.game.GetGroup(GameMatcher
                .AnyOf(
                    GameMatcher.TriggeredBy,
                    GameMatcher.TriggerCollider));
        }

        public void Cleanup()
        {
            foreach (GameEntity entity in _triggered.GetEntities())
                ResetCollided(entity);
        }

        private void ResetCollided(GameEntity entity)
        {
            if (entity.hasTriggerCollider)
                entity.RemoveTriggerCollider();

            if (entity.hasTriggeredBy)
                entity.RemoveTriggeredBy();
        }
    }
}