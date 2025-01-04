using System.Collections.Generic;
using Code.Common;
using Entitas;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class RemoveAbilityWithoutTargetSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _effects;
        private readonly List<GameEntity> _buffer = new(128);

        public RemoveAbilityWithoutTargetSystem(GameContext game)
        {
            _effects = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Effect,
                    GameMatcher.Target));
        }

        public void Execute()
        {
            foreach (GameEntity effect in _effects.GetEntities(_buffer))
            {
                GameEntity target = effect.Target();
                
                if (target.IsNullOrDestructed())
                    effect.Destroy();
            }
        }
    }
}