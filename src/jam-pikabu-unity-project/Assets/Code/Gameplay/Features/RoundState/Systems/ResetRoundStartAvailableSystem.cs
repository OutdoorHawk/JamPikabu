using Entitas;

namespace Code.Gameplay.Features.RoundState.Systems
{
    public class ResetRoundStartAvailableSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _entities;

        public ResetRoundStartAvailableSystem(GameContext context)
        {
            _entities = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.RoundStateController
                ));
        }
        
        public void Cleanup()
        {
            foreach (var entity in _entities)
            {
                entity.isRoundStartAvailable = true;
            }
        }
    }
}