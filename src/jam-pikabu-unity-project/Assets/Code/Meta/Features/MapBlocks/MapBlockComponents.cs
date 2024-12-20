using Code.Progress;
using Entitas;

namespace Code.Meta.Features.MapBlocks
{
    [Meta] public sealed class ReadyToFreeUpgrade : IComponent { } 
    [Meta] public sealed class FreeUpgradeRequest : IComponent { } 
    [Meta] public sealed class LootFreeUpgradeTimer : ISavedComponent { } 
    [Meta] public sealed class NextFreeUpgradeTime : ISavedComponent { public int Value; } 
}