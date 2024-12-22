using System;
using Code.Gameplay.StaticData.Data;
using Sirenix.OdinInspector;

namespace Code.Meta.Features.Days.Configs.Stars
{
    [Serializable]
    public class DayStarsSetup : BaseData
    {
        public int RatingNeedAll;

#if UNITY_EDITOR
        [ReadOnly] public float AverageMinRatingPerDay;
        [ ReadOnly] public float AverageMaxRatingPerDay;
#endif
    }
}