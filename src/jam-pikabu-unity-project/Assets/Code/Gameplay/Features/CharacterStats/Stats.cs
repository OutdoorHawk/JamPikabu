﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Code.Gameplay.Features.CharacterStats
{
    public enum Stats
    {
        Unknown = 0,
        Speed = 1,
        Scale = 2,
    }

    public static class InitStats
    {
        public static Dictionary<Stats, float> EmptyStatDictionary()
        {
            return Enum.GetValues(typeof(Stats))
                .Cast<Stats>()
                .Except(new[] { Stats.Unknown })
                .ToDictionary(x => x, _ => 0f);
        }
        
    }
}