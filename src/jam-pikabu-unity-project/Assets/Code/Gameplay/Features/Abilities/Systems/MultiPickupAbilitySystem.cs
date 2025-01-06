using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class MultiPickupAbilitySystem : ReactiveSystem<GameEntity>
    {
        private readonly IGroup<GameEntity> _abilities;
        private readonly IGroup<GameEntity> _markedForPickupLoot;

        public MultiPickupAbilitySystem(GameContext context) : base(context)
        {
            _abilities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.MultiPickupAbility,
                    GameMatcher.Ability,
                    GameMatcher.Target
                ));

            _markedForPickupLoot = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.MarkedForPickup,
                    GameMatcher.Loot,
                    GameMatcher.Rigidbody2D
                ));
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.CollectLootRequest.Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isLoot && entity.isCollectLootRequest && entity.hasAbilityVisuals;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var ability in _abilities)
            foreach (var loot in entities)
            {
                if (loot.Id != ability.Target)
                    continue;

                if (_markedForPickupLoot.count == 1)
                {
                    loot.isCollectLootRequest = false;
                    loot.AbilityVisuals.PlayWrongCollection();
                }
            }
        }
    }
}