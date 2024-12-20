using System;
using System.Collections.Generic;
using Code.Gameplay.StaticData.Data;
using Sirenix.OdinInspector;

namespace Code.Meta.Features.Days.Configs.Stars
{
    [Serializable]
    public class DayStarsSetup : BaseData
    {
        [FoldoutGroup("Data")] public List<DayStarData> Stars = new() { new DayStarData(), new DayStarData(), new DayStarData() };

#if UNITY_EDITOR
        [FoldoutGroup("Editor"), ReadOnly] public float AverageMinRatingPerDay;
        [FoldoutGroup("Editor"), ReadOnly] public float AverageMaxRatingPerDay;
#endif
    }
}