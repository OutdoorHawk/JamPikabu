using System;
using Code.Gameplay.StaticData.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Meta.Features.Days.Configs.Stars
{
    [Serializable]
    public class DayStarsSetup : BaseData
    {
        public int RatingNeedAll;

#if UNITY_EDITOR
        [ReadOnly, HideInInspector] public float AverageMinRatingPerDay;
        [ ReadOnly, HideInInspector] public float AverageMaxRatingPerDay;
#endif
    }
}