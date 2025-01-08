using System.Collections.Generic;
using Code.Gameplay.Features.Cooldowns;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class HeavyObjectAbilitySystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _abilities;
        private readonly IGroup<GameEntity> _lootInsideHook;
        private readonly List<GameEntity> _buffer = new(32);
        private readonly IGroup<GameEntity> _hook;

        public HeavyObjectAbilitySystem(GameContext context)
        {
            _abilities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.HeavyObjectAbility,
                    GameMatcher.Ability,
                    GameMatcher.Target,
                    GameMatcher.HeavyObjectSpeedFactor
                ));
            
            _lootInsideHook = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Loot,
                    GameMatcher.InsideHook,
                    GameMatcher.Rigidbody2D
                ));
            
            _hook = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.GrapplingHook,
                    GameMatcher.HookSpeedModifier,
                    GameMatcher.View
                ));
        }

        public void Execute()
        {
            foreach (var loot in _lootInsideHook)
            foreach (var ability in _abilities.GetEntities(_buffer))
            {
                GameEntity target = ability.Target();

                if (target.Id != loot.Id)
                    continue;

                foreach (GameEntity hook in _hook)
                {
                    //hook.ReplaceHookSpeedModifier();
                }
            }
        }
    }
}