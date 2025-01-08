using Code.Common.Extensions;
using Entitas;

namespace Code.Gameplay.Features.CharacterStats.Systems
{
    public class ApplySpeedFromStatsSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _statOwners;

        public ApplySpeedFromStatsSystem(GameContext game)
        {
            _statOwners = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.BaseStats,
                    GameMatcher.StatModifiers,
                    GameMatcher.Speed));
        }

        public void Execute()
        {
            foreach (GameEntity statOwner in _statOwners)
            {
                statOwner.ReplaceSpeed(MoveSpeed(statOwner).ZeroIfNegative());
            }
        }

        private static float MoveSpeed(GameEntity statOwner)
        {
            if (statOwner.BaseStats.TryGetValue(Stats.Speed, out float baseStatValue) == false)
                return statOwner.Speed;

            return baseStatValue + statOwner.StatModifiers[Stats.Speed];
        }
    }
}