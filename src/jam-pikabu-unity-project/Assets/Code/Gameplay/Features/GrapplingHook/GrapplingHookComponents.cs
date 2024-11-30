
using Code.Gameplay.Features.GrapplingHook.Behaviours;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.GrapplingHook
{
    [Game] public sealed class GrapplingHook : IComponent { }
    [Game] public sealed class XAxisSpeed : IComponent { public float Value; }
    [Game] public sealed class YAxisDownSpeed : IComponent { public float Value; }
    [Game] public sealed class YAxisUpSpeed : IComponent { public float Value; }
    [Game] public sealed class StopMovementRaycastRadius : IComponent { public float Value; }
    [Game] public sealed class CollectLootRaycastRadius : IComponent { public float Value; }
    [Game] public sealed class XMovementLimits : IComponent { public Vector2 Value; }
    
    [Game] public sealed class XAxisMovementAvailable : IComponent {}
    [Game] public sealed class YAxisMovementAvailable : IComponent {}
    [Game] public sealed class XAxisMoveDirection : IComponent { public float Value; }
    
    [Game] public sealed class GrapplingHookBehaviourComponent : IComponent { public GrapplingHookBehaviour Value; }
    [Game] public sealed class DescentRequested : IComponent {}
    [Game] public sealed class DescentAvailable : IComponent {}
    [Game] public sealed class AscentRequested : IComponent {}
    [Game] public sealed class AscentAvailable : IComponent {}
    [Game] public sealed class CollectLootRequest : IComponent {}
}