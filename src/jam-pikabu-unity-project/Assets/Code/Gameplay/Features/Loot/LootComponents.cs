using Entitas;

namespace Code.Gameplay.Features.Loot
{
    [Game] public sealed class Loot : IComponent { }
    [Game] public sealed class LootTypeIdComponent : IComponent { public LootTypeId Value; }
}