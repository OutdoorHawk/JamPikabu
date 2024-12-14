using Entitas;

namespace Code.Gameplay.Features.GameState
{
    [Game] public sealed class GameStateTypeIdComponent: IComponent { public GameStateTypeId Value; }
    [Game] public sealed class BeginDay : IComponent { }
    [Game] public sealed class RoundPreparation : IComponent { }
    [Game] public sealed class RoundLoop : IComponent { }
    [Game] public sealed class RoundCompletion : IComponent { }
    [Game] public sealed class EndDay : IComponent { }
    [Game] public sealed class SwitchGameStateRequest : IComponent { }
    [Game] public sealed class GameState : IComponent { }
    [Game] public sealed class StateProcessingAvailable : IComponent { }
}