using System;
using System.Collections.Generic;
using Code.Infrastructure.Common.Time;
using UnityEngine;

namespace Code.Gameplay.Common.Time
{
    public class UnityTimeService : ITimeService
    {
        private readonly List<IPauseHandler> _handlers = new();

        public const string CheatTimeOffsetKey = "cheat_delta_time";

        public bool IsPaused { get; private set; }
        public float CurrentTimeScale { get; private set; } = 1;

        public float DeltaTime => !IsPaused ? UnityEngine.Time.deltaTime : 0;
        public float FixedDeltaTime => !IsPaused ? UnityEngine.Time.fixedDeltaTime : 0;
        
        public DateTime UtcNow => DateTime.UtcNow;
        
        public DateTime EpochStart { get; } = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); 
        
        public int TimeOffset { get; set; }
        
        public int TimeStamp 
        {
            get {
                var time = (DateTime.UtcNow - EpochStart).TotalSeconds + DeltaTime;
#if CHEAT
                time += TimeOffset;
#endif
                return (int)time;
            } 
        }

        public UnityTimeService()
        {
#if CHEAT
            if (PlayerPrefs.HasKey(CheatTimeOffsetKey)) 
                TimeOffset = PlayerPrefs.GetInt(CheatTimeOffsetKey);
#endif
        }

        public void Register(IPauseHandler handler)
        {
            _handlers.Add(handler);
        }

        public void UnRegister(IPauseHandler handler)
        {
            _handlers.Remove(handler);
        }

        public void EnablePause()
        {
            IsPaused = true;
            foreach (IPauseHandler handler in _handlers)
                handler.EnablePause();
            CurrentTimeScale = 0;
        }

        public void DisablePause()
        {
            IsPaused = false;
            foreach (IPauseHandler handler in _handlers)
                handler.DisablePause();
            CurrentTimeScale = 1;
        }
    }
}