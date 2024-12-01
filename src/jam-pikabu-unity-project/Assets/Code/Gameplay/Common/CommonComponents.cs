using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Code.Gameplay.Common
{
  [Game, Meta] public sealed class Id : IComponent { [PrimaryEntityIndex] public int Value; }
  
  [Game] public sealed class Rigidbody2DComponent : IComponent { public Rigidbody2D Value; }
  
  [Game] public class CollisionId : IComponent { [EntityIndex] public int Value; }

  [Game] public sealed class WorldPosition : IComponent { public Vector3 Value; }
  [Game] public sealed class StartWorldPosition : IComponent { public Vector3 Value; }
  [Game] public sealed class StartRotation : IComponent { public Vector3 Value; }
 
  [Game] public sealed class TransformComponent : IComponent { public Transform Value; }

}