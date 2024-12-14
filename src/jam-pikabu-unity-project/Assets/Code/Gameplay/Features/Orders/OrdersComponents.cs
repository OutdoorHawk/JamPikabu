using Code.Gameplay.Features.Orders.Config;
using Entitas;

namespace Code.Gameplay.Features.Orders
{
    [Game] public sealed class OrderDataComponent : IComponent { public OrderData Value; }
    [Game] public sealed class Order : IComponent { }
    [Game] public sealed class Reject : IComponent { }
    [Game] public sealed class Complete : IComponent { }
    [Game] public sealed class BossOrder : IComponent { }
    [Game] public sealed class ResultProcessed : IComponent { }
    [Game] public sealed class NextOrderRequest : IComponent { }
}