using Code.Progress;
using Entitas;

namespace Code.Meta.Features.ExpirationTimer
{
    [Meta] public sealed class ExpirationTime : ISavedComponent { public int Value; } 
    [Meta] public sealed class Expired : IComponent { } 
}