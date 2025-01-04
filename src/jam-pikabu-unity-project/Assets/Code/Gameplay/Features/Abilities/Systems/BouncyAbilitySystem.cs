using Code.Gameplay.Features.Cooldowns;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class BouncyAbilitySystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _abilities;

        public BouncyAbilitySystem(GameContext context)
        {
            _abilities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.BouncyAbility,
                    GameMatcher.Target,
                    GameMatcher.Rigidbody2D,
                    GameMatcher.CooldownUp
                ));
        }

        public void Execute()
        {
            foreach (var ability in _abilities)
            {
                Rigidbody2D rigidbody = ability.Rigidbody2D;
                rigidbody.AddForce(new Vector2(0f, ability.BounceStrength), ForceMode2D.Impulse);

                ability.PutOnCooldown();
            }
        }
    }
}