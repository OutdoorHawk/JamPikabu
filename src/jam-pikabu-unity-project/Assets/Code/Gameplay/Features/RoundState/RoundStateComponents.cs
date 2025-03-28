﻿using Code.Gameplay.Features.RoundState.Behaviours;
using Entitas;

namespace Code.Gameplay.Features.RoundState
{
    [Game] public sealed class RoundStateController : IComponent { }
    [Game] public sealed class RoundStartRequest : IComponent { }
    [Game] public sealed class RoundStartAvailable : IComponent { }
    [Game] public sealed class RoundInProcess : IComponent { }
    [Game] public sealed class RoundOver : IComponent { }
    [Game] public sealed class RoundComplete : IComponent { }
    [Game] public sealed class GameOver : IComponent { }
    [Game] public sealed class DayCompleteRequest : IComponent { }
    [Game] public sealed class RoundTimeLeft : IComponent { public float Value;  }
    [Game] public sealed class RoundDuration : IComponent { public float Value; }
    [Game] public sealed class Round : IComponent { public int Value; }
    [Game] public sealed class HookAttemptsLeft : IComponent { public int Value; }
    [Game] public sealed class HookAttemptsMax : IComponent { public int Value; }
    [Game] public sealed class RoundStateViewBehaviourComponent : IComponent { public RoundStateViewBehaviour Value; }
    [Game] public sealed class RoundEndRequest : IComponent { }
}