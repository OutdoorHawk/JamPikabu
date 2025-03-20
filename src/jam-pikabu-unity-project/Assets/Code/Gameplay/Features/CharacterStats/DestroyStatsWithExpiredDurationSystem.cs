using Entitas;

namespace Code.Gameplay.Features.CharacterStats
{
    public class DestroyStatsWithExpiredDurationSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _stats;

        public DestroyStatsWithExpiredDurationSystem(GameContext context)
        {
            _stats = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.StatChange,
                    GameMatcher.EffectValue,
                    GameMatcher.AbilityDuration));
        }

        public void Cleanup()
        {
            foreach (var entity in _stats)
            {
                if (entity.AbilityDuration <= 0) 
                    entity.isDestructed = true;
            }
        }
    }
}