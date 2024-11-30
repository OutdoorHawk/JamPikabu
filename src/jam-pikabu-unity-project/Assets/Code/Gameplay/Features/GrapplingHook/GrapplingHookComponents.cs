
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
}