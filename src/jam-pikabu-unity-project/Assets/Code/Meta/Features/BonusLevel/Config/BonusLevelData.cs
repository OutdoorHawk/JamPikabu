using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData.Data;
using Code.Infrastructure.SceneLoading;

namespace Code.Meta.Features.BonusLevel.Config
{
    [Serializable]
    public class BonusLevelData : BaseData
    {
        public BonusLevelType Type;
        public List<SceneTypeId> SceneTypeId;
        public int ResetTimeMinutes;
        public float GoldFactorModifier = 2;
        public int RoundTimeOverride = 45;
        public List<LootTypeId> AvailableIngredients;

        public int ResetTimeSeconds => ResetTimeMinutes * 60;
    }
}