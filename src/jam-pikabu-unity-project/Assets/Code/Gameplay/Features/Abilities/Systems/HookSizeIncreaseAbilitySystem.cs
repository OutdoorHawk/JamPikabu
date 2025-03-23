using Code.Common.Entity;
using Code.Gameplay.Features.CharacterStats;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class HookSizeIncreaseAbilitySystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _abilities;
        private readonly IGroup<GameEntity> _collectingLoot;
        private readonly IGroup<GameEntity> _hook;

        public HookSizeIncreaseAbilitySystem(GameContext context)
        {
            _abilities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.IncreaseHookSizeAbility,
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
                
                foreach (GameEntity hook in _hook)
                {
                    TryApplyAbility(ability, hook);
                }
            }
        }

        private static void TryApplyAbility(GameEntity ability, GameEntity hook)
        {
            if (ability.ActivationChance < Random.Range(0, 101))
                return;
            
            CreateGameEntity.Empty()
                .AddStatChange(Stats.Scale)
                .AddEffectValue(ability.AbilityEffectValue)
                .AddTarget(hook.Id)
                .AddProducer(ability.Id)
                .AddAbilityDuration((int)ability.AbilityDuration);
        }
    }
}