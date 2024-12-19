using Code.Progress;
using Entitas;

namespace Code.Meta.Features.Storage
{
    [Meta] public sealed class Storage : ISavedComponent { }
    [Meta] public sealed class Withdraw : IComponent { public int Value; }
    [Meta] public sealed class AddCurrencyToStorageRequest : ISavedComponent { }
    [Meta] public sealed class Gold : ISavedComponent { public int Value; }
    
}