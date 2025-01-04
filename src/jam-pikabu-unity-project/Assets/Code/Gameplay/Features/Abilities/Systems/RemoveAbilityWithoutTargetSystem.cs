using System.Collections.Generic;
using Code.Common;
using Entitas;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class RemoveAbilityWithoutTargetSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _abilities;
        private readonly List<GameEntity> _buffer = new(128);

        public RemoveAbilityWithoutTargetSystem(GameContext game)
        {
            _abilities = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Effect,
                    GameMatcher.Target));
        }

        public void Execute()
        {
            foreach (GameEntity ability in _abilities.GetEntities(_buffer))
            {
                GameEntity target = ability.Target();
                
                if (target.IsNullOrDestructed())
                {
                    ability.isAbility = false;
                    ability.RemoveTarget();
                    ability.Destroy();
                }
            }
        }
    }
}