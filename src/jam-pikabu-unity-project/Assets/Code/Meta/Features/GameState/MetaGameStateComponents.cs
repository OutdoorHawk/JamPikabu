using Entitas;

namespace Code.Meta.Features.GameState
{
    [Meta] public sealed class Day : IComponent { public int Value; }
    [Meta] public sealed class Gold : IComponent { public int Value; }
    [Meta] public sealed class Minus : IComponent { public int Value; }
    [Meta] public sealed class Plus : IComponent { public int Value; }
}