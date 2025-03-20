using Code.Common.Extensions;
using Entitas;

namespace Code.Gameplay.Features.CharacterStats.Systems
{
    public class ApplyScaleFromStatsSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _statOwners;

        public ApplyScaleFromStatsSystem(GameContext game)
        {
            _statOwners = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.BaseStats,
                    GameMatcher.StatModifiers,
                    GameMatcher.Scale));
        }

        public void Execute()
        {
            foreach (GameEntity statOwner in _statOwners)
            {
                statOwner.ReplaceScale(ApplyModificator(statOwner, Stats.Scale).ZeroIfNegative());
            }
        }

        private static float ApplyModificator(GameEntity statOwner, Stats stat)
        {
            if (statOwner.BaseStats.TryGetValue(stat, out float baseStatValue) == false)
                return statOwner.Scale;

            return baseStatValue + statOwner.StatModifiers[stat];
        }
    }
}