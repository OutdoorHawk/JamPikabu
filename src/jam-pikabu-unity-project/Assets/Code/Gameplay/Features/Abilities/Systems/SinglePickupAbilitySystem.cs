using System.Collections.Generic;
using System.Linq;
using Entitas;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class SinglePickupAbilitySystem : ReactiveSystem<GameEntity>
    {
        private readonly IGroup<GameEntity> _abilities;
        private readonly IGroup<GameEntity> _markedForPickupLoot;

        public SinglePickupAbilitySystem(GameContext context) : base(context)
        {
            _abilities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.SinglePickupAbility,
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
            return context.CreateCollector(GameMatcher.MarkedForPickup.Added());
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

                if (_markedForPickupLoot.count < 1)
                    continue;

                List<GameEntity> sameTypeIngredients = _markedForPickupLoot
                    .GetEntities()
                    .ToList()
                    .FindAll(x => x.LootTypeId == loot.LootTypeId);

                if (sameTypeIngredients.Count > 1)
                {
                    foreach (var sameType in sameTypeIngredients)
                    {
                        sameType.isCollectLootRequest = false;
                        sameType.isMarkedForPickup = false;
                        sameType.AbilityVisuals.PlayWrongCollection();
                    }
                }
            }
        }
    }
}