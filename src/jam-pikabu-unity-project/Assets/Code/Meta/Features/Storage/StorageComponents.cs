using Code.Progress;
using Entitas;

namespace Code.Meta.Features.Storage
{
    [Meta] public sealed class Storage : ISavedComponent { }
    [Meta] public sealed class Withdraw : ISavedComponent { public int Value; }
    [Meta] public sealed class AddCurrencyToStorageRequest : ISavedComponent { }
    
    [Meta] public sealed class Gold : ISavedComponent { public int Value; }
    [Meta] public sealed class Minus : ISavedComponent { public int Value; }
    [Meta] public sealed class Plus : ISavedComponent { public int Value; }
    
}