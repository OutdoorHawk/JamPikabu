using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Entitas;
using UnityEngine;
using static Code.Gameplay.Features.Abilities.AbilityExtensions;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class PickupRandomLootAbilitySystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _abilities;
        private readonly IGroup<GameEntity> _collectingLoot;
        private readonly IGroup<GameEntity> _otherLoot;

        private readonly List<GameEntity> _otherLootBuffer = new(16);
        private readonly List<GameEntity> _lootBuffer = new(32);

        public PickupRandomLootAbilitySystem(GameContext context)
        {
            _abilities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.PickupRandomLootAbility,
                    GameMatcher.Ability,
                    GameMatcher.Target
                ));

            _collectingLoot = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Loot,
                    GameMatcher.CollectLootRequest
                ));

            _otherLoot = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Loot,
                    GameMatcher.View)
                .NoneOf(GameMatcher.CollectLootRequest,
                    GameMatcher.Collected));
        }

        public void Execute()
        {
            foreach (var loot in _collectingLoot.GetEntities(_lootBuffer))
            foreach (var ability in _abilities)
            {
                if (loot.Id != ability.Target)
                    continue;

                if (_otherLoot.count == 0)
                    continue;

                _otherLoot.GetEntities(_otherLootBuffer);
                GameEntity randomLoot = GetRandomLoot(loot, _otherLootBuffer, x => Match(x, loot.LootTypeId));
                randomLoot.isCollectLootRequest = true;
            }
        }

        private bool Match(GameEntity loot, LootTypeId typeToExclude)
        {
            if (loot.LootTypeId == typeToExclude)
                return false;

            return true;
        }
    }
}