using System;
using Code.Infrastructure.Common.Time;

namespace Code.Gameplay.Common.Time
{
    public interface ITimeService : IPauseHandler
    {
        bool IsPaused { get; }
        float CurrentTimeScale { get; }
        float DeltaTime { get; }
        float FixedDeltaTime { get; }
        DateTime UtcNow { get; }
        float Time { get; }

        void Register(IPauseHandler handler);
        void UnRegister(IPauseHandler handler);
    }
}