﻿using System.Collections.Generic;
using Code.Common;
using Code.Gameplay.Features.Abilities.Behaviours;
using Code.Gameplay.Features.Cooldowns;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Entitas;
using static Code.Gameplay.Features.Abilities.AbilityExtensions;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class SwapPositionsAbilitySystem : IExecuteSystem
    {
        private readonly ISoundService _soundService;
        private readonly IGroup<GameEntity> _abilities;
        private readonly List<GameEntity> _buffer = new(64);
        private readonly List<GameEntity> _lootBuffer = new(64);
        private readonly IGroup<GameEntity> _loot;

        public SwapPositionsAbilitySystem(GameContext context, ISoundService soundService)
        {
            _soundService = soundService;
            _abilities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.SwapPositionsAbility,
                    GameMatcher.Ability,
                    GameMatcher.Target,
                    GameMatcher.CooldownUp
                ));

            _loot = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Loot,
                    GameMatcher.Rigidbody2D
                ).NoneOf(
                    GameMatcher.Collected,
                    GameMatcher.Consumed));
        }

        public void Execute()
        {
            foreach (var ability in _abilities.GetEntities(_buffer))
            {
                ability.PutOnCooldown();

                GameEntity target = ability.Target();

                if (target.hasAbilityVisuals == false)
                    continue;

                _loot.GetEntities(_lootBuffer);

                GameEntity randomLoot = GetRandomLoot(target, _lootBuffer, Match);

                if (randomLoot.IsNullOrDestructed())
                    continue;

                AbilityVisuals targetVisuals = target.AbilityVisuals;
                AbilityVisuals randomLootVisuals = randomLoot.AbilityVisuals;

                targetVisuals.PlaySwap(newPosition: randomLootVisuals.transform.position);
                randomLootVisuals.PlaySwap(newPosition: targetVisuals.transform.position);
                
                _soundService.PlaySound(SoundTypeId.PositionChangeAbility);
            }
        }

        private bool Match(GameEntity loot)
        {
            if (loot.hasAbilityVisuals == false)
                return false;

            return true;
        }
    }
}