using Code.Gameplay.Features.Abilities.Systems;
using Code.Infrastructure.Systems;

namespace Code.Gameplay.Features.Abilities
{
    public sealed class AbilityFeature : Feature
    {
        public AbilityFeature(ISystemFactory systems)
        {
            Add(systems.Create<RemoveAbilityWithoutTargetSystem>());

            Add(systems.Create<BouncyAbilitySystem>());
            Add(systems.Create<SwapPositionsAbilitySystem>());
            Add(systems.Create<ChangeIngredientSizesAbilitySystem>());
            Add(systems.Create<HookSpeedChangeAbilitySystem>());
            Add(systems.Create<PickupRandomLootAbilitySystem>());
            Add(systems.Create<SinglePickupAbilitySystem>());
            Add(systems.Create<MultiPickupAbilitySystem>());
            Add(systems.Create<HeavyObjectAbilitySystem>());
            Add(systems.Create<StickyToHookAbilitySystem>());
            Add(systems.Create<HookSizeIncreaseAbilitySystem>());
            Add(systems.Create<HookSizeDecreaseAbilitySystem>());
        }
    }
}