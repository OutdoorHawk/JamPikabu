using Code.Gameplay.Features.Loot;
using Code.Progress;
using Entitas;

namespace Code.Meta.Features.LootCollection
{
    [Meta] public sealed class LootProgression : ISavedComponent {  }
    [Meta] public sealed class LootTypeIdComponent : ISavedComponent { public LootTypeId Value; }
    [Meta] public sealed class Level : ISavedComponent { public int Value; }
    [Meta] public sealed class UpgradeLootRequest : IComponent { } 
    [Meta] public sealed class UnlockLootRequest : IComponent { } 

}