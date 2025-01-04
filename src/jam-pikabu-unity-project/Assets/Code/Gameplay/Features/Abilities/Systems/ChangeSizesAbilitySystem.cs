using System.Collections.Generic;
using Code.Gameplay.Features.Cooldowns;
using Entitas;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class ChangeSizesAbilitySystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _abilities;
        private readonly List<GameEntity> _buffer = new(64);

        public ChangeSizesAbilitySystem(GameContext context)
        {
            _abilities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.ChangeSizesAbility,
                    GameMatcher.Ability,
                    GameMatcher.Target,
                    GameMatcher.CooldownUp
                ));
        }

        public void Execute()
        {
            foreach (var ability in _abilities.GetEntities(_buffer))
            {
                ability.PutOnCooldown();

                GameEntity target = ability.Target();

                if (target.hasAbilityVisuals == false)
                    continue;

                target.AbilityVisuals.ChangeSize();
            }
        }
    }
}