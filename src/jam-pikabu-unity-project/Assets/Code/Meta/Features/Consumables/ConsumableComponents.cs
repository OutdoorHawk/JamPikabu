using Code.Progress;
using Entitas;

namespace Code.Meta.Features.Consumables
{
    [Meta] public sealed class ActiveExtraLoot : ISavedComponent { }
    [Meta] public sealed class ConsumableTypeIdComponent : ISavedComponent { public ConsumableTypeId Value; }
    [Meta] public sealed class UpdateConsumableRequest : IComponent { }
}