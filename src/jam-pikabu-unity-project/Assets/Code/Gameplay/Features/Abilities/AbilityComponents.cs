using Code.Gameplay.Features.Abilities.Behaviours;
using Entitas;

namespace Code.Gameplay.Features.Abilities
{
    [Game] public sealed class Ability : IComponent { }
    [Game] public sealed class AbilityType : IComponent { public AbilityTypeId Value; }
    [Game] public sealed class AbilityVisualsComponent : IComponent { public AbilityVisuals Value; }
    [Game] public sealed class AbilityDuration : IComponent { public float Value; }
    [Game] public sealed class ActivationChance : IComponent { public int Value; }
    
    [Game] public sealed class BouncyAbility : IComponent { }
    [Game] public sealed class BounceStrength : IComponent { public float Value; }
    
    [Game] public sealed class SwapPositionsAbility : IComponent { }
    
    [Game] public sealed class ChangeSizesAbility : IComponent { }
    
    [Game] public sealed class HookSpeedChangeAbility : IComponent { }
    [Game] public sealed class SpeedChangeAmount : IComponent { public float Value; }
    
    [Game] public sealed class PickupRandomLootAbility : IComponent { }
    
    [Game] public sealed class SinglePickupAbility : IComponent { }
    
    [Game] public sealed class MultiPickupAbility : IComponent { }
    
    [Game] public sealed class HeavyObjectAbility : IComponent { }
    [Game] public sealed class HeavyObjectSpeedFactor : IComponent { public float Value; }
    
    [Game] public sealed class StickyToHookAbility : IComponent { }
}