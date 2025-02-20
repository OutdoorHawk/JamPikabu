using Code.Meta.Features.Consumables;
using Entitas;

namespace Code.Gameplay.Features.Consumables
{
    [Game] public sealed class ActivateConsumableRequest : IComponent {  }
    [Game] public sealed class ConsumableTypeIdComponent : IComponent { public ConsumableTypeId Value; }
    [Game] public sealed class Spoon : IComponent {  }
    [Game] public sealed class Wood : IComponent {  }
    [Game] public sealed class Processed : IComponent {  }
}