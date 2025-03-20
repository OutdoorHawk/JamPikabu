using Code.Common.Extensions;
using Code.Gameplay.Features.Abilities.Behaviours;
using Entitas;
using Zenject;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class StickyToHookAbilitySystem : IExecuteSystem
    {
        private readonly IInstantiator _instantiator;
        private readonly IGroup<GameEntity> _abilities;

        public StickyToHookAbilitySystem(GameContext context, IInstantiator instantiator)
        {
            _instantiator = instantiator;
            _abilities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.StickyToHookAbility,
                    GameMatcher.Ability,
                    GameMatcher.Target
                ));
        }

        public void Execute()
        {
            foreach (var ability in _abilities)
            {
                GameEntity target = ability.Target();

                if (target.hasView == false)
                    continue;
                
                var component = _instantiator.InstantiateComponent<StickToKinematic>(target.View.gameObject);
                component.StickLayerMask = CollisionLayer.Hook.AsMask();
                target.AddStickToKinematic(component);
                ability.isDestructed = true;
            }
        }
    }
}