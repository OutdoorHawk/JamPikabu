using Code.Gameplay.Common.EntityIndices;
using Entitas;

namespace Code.Gameplay.Features.CharacterStats.Systems
{
    public class StatChangeSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _statOwners;
        private readonly GameContext _game;

        public StatChangeSystem(GameContext game)
        {
            _game = game;

            _statOwners = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Id,
                    GameMatcher.BaseStats,
                    GameMatcher.StatModifiers));
        }
        
        public void Execute()
        {
            foreach (GameEntity statOwner in _statOwners)
            {
                var baseStats = statOwner.BaseStats;
                var statModifiers = statOwner.StatModifiers;

                foreach (Stats stat in baseStats.Keys)
                {
                    statModifiers[stat] = 0;
                }

                foreach (Stats stat in baseStats.Keys)
                foreach (GameEntity statChange in _game.TargetStatChanges(stat, statOwner.Id))
                {
                    if (statChange.isMultiplicative)
                    {
                        float modifiedValue = baseStats[stat] * statChange.EffectValue;
                        statModifiers[stat] += modifiedValue;
                    }
                    else
                    {
                        statModifiers[stat] += statChange.EffectValue;
                    }
                }
            }
        }
    }
}