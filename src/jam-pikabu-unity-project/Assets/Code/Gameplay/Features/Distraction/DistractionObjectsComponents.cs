using Code.Gameplay.Features.Distraction.Behaviours;
using Entitas;

namespace Code.Gameplay.Features.Distraction
{
    [Game] public sealed class Bee : IComponent { }
    [Game] public sealed class DistractionObject : IComponent { }
    [Game] public sealed class DistractionObjectTypeIdComponent : IComponent { public DistractionObjectTypeId Value; }
    [Game] public sealed class BeeBehaviourComponent : IComponent { public BeeBehaviour Value; }
  
}