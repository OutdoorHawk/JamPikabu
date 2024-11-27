using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.CollidingView
{
    [Game] public sealed class CollisionComponent : IComponent { public Collision Value; }
    [Game] public sealed class ColliderComponent : IComponent { public Collider Value; }
    [Game] public sealed class CollidedByComponent : IComponent { public int Value; }
    
    [Game] public sealed class TriggerCollider : IComponent { public Collider Value; }
    [Game] public sealed class TriggeredByComponent : IComponent { public int Value; }
    
}