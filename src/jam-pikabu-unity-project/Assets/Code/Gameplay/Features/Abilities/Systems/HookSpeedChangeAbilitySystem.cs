using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class HookSpeedChangeAbilitySystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _abilities;
        private readonly IGroup<GameEntity> _collectingLoot;
        private readonly IGroup<GameEntity> _hook;

        public HookSpeedChangeAbilitySystem(GameContext context)
        {
            _abilities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.HookSpeedChangeAbility,
                    GameMatcher.Ability,
                    GameMatcher.Target
                ));

            _collectingLoot = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Loot,
                    GameMatcher.CollectLootRequest
                ));

            _hook = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook,
                    GameMatcher.GrapplingHookBehaviour
                ));
        }

        public void Execute()
        {
            foreach (var loot in _collectingLoot)
            foreach (var ability in _abilities)
            {
                if (loot.Id != ability.Target)
                    continue;

                if (Random.Range(0, 101) > ability.ActivationChance)
                    continue;

                foreach (GameEntity hook in _hook)
                {
                    hook.GrapplingHookBehaviour.ApplySpeedChange(factor: ability.SpeedChangeAmount, duration: ability.AbilityDuration);
                }
            }
        }
    }
}