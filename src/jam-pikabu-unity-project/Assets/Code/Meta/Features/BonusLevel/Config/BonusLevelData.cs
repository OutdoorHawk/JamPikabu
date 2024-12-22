using System;
using Code.Gameplay.StaticData.Data;

namespace Code.Meta.Features.BonusLevel.Config
{
    [Serializable]
    public class BonusLevelData : BaseData
    {
        public BonusLevelType Type;
        public int ResetTimeMinutes;
        
        public int ResetTimeSeconds => ResetTimeMinutes * 60;
    }
}