using System.Collections.Generic;
using Code.Common.Entity;
using Code.Gameplay.Features.CharacterStats;
using Entitas;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class HeavyObjectAbilitySystem : IExecuteSystem, ICleanupSystem
    {
        private readonly IGroup<GameEntity> _abilities;
        private readonly IGroup<GameEntity> _lootInsideHook;
        private readonly IGroup<GameEntity> _hook;

        private readonly List<GameEntity> _buffer = new(32);
        private readonly IGroup<GameEntity> _stats;

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

            _stats = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.StatChange,
                    GameMatcher.EffectValue,
                    GameMatcher.Target,
                    GameMatcher.Producer
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

                if (loot.Rigidbody2D.gravityScale <= 0)
                    continue;

                foreach (GameEntity hook in _hook)
                {
                    CreateGameEntity.Empty()
                        .AddStatChange(Stats.Speed)
                        .AddEffectValue(ability.HeavyObjectSpeedFactor)
                        .AddTarget(hook.Id)
                        .AddProducer(ability.Id)
                        ;

                    hook.GrapplingHookBehaviour.ShowHeavyParticles();
                }
            }
        }

        public void Cleanup()
        {
            foreach (GameEntity stat in _stats)
            foreach (GameEntity ability in _abilities)
            {
                if (stat.Producer != ability.Id)
                    continue;

                stat.isDestructed = true;
            }
        }
    }
}