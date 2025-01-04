using Entitas;

namespace Code.Gameplay.Features.Abilities
{
    [Game] public sealed class Ability : IComponent { }
    [Game] public sealed class BouncyAbility : IComponent { }
    [Game] public sealed class BounceStrength : IComponent { public float Value; }
    
}