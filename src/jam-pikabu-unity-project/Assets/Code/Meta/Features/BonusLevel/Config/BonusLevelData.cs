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
        public int FreePlayResetTimeMinutes = 300;
        public float GoldFactorModifier = 2;
        public int RoundTimeOverride = 45;
        public int MainLevelsIngredientsAmount = 2;
        public List<LootTypeId> AvailableIngredients;

        public int ResetTimeSeconds => FreePlayResetTimeMinutes * 60;
    }
}