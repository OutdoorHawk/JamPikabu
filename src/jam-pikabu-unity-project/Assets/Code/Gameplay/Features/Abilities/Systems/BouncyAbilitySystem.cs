using System.Collections.Generic;
using Code.Gameplay.Features.Cooldowns;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class BouncyAbilitySystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _abilities;
        private readonly List<GameEntity> _buffer = new(64);

        public BouncyAbilitySystem(GameContext context)
        {
            _abilities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.BouncyAbility,
                    GameMatcher.Ability,
                    GameMatcher.Target,
                    GameMatcher.CooldownUp
                ));
        }

        public void Execute()
        {
            foreach (var ability in _abilities.GetEntities(_buffer))
            {
                GameEntity target = ability.Target();

                if (target.hasRigidbody2D == false)
                    continue;

                ability.PutOnCooldown();

                Rigidbody2D rigidbody = target.Rigidbody2D;
                rigidbody.AddForce(new Vector2(0f, ability.BounceStrength), ForceMode2D.Impulse);
            }
        }
    }
}