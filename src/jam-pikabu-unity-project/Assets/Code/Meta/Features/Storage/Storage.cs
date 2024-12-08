using Code.Progress;
using Entitas;

namespace Code.Meta.Features.Storage
{
    [Meta] public sealed class Storage : IComponent { }
    [Meta] public sealed class AddCurrencyToStorageRequest : ISavedComponent { }
    [Meta] public sealed class Hard : ISavedComponent { public int Value; }
    
}