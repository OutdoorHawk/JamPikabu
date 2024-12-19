using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData.Data;
using Code.Infrastructure.SceneLoading;
using Sirenix.OdinInspector;

namespace Code.Meta.Features.Days.Configs
{
    [Serializable]
    public class DayData : BaseData
    {
        public int StarsNeedToUnlock;
        public LootTypeId UnlocksIngredient;
        public bool IsBossDay;
        [FoldoutGroup("Data")] public int OrdersAmount = 3;
        [FoldoutGroup("Data")] public float DayGoldFactor = 1;
        [FoldoutGroup("Data")] public SceneTypeId SceneId = SceneTypeId.Level_1;
        [FoldoutGroup("Data")] public List<DayStarData> Stars = new() { new DayStarData(), new DayStarData(), new DayStarData() };

#if UNITY_EDITOR
        [FoldoutGroup("Editor"), ReadOnly] public float AverageMinRatingPerDay;
        [FoldoutGroup("Editor"), ReadOnly] public float AverageMaxRatingPerDay;
#endif
    }
}