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
        DateTime EpochStart { get; }
        int TimeOffset { get; set; }
        int TimeStamp { get; }
        void Register(IPauseHandler handler);
        void UnRegister(IPauseHandler handler);
        void EnablePause();
        void DisablePause();
    }
}