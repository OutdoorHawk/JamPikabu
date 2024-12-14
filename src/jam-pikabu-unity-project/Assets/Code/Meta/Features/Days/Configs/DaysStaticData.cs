using System.Collections.Generic;
using Code.Gameplay.StaticData;
using UnityEngine;

namespace Code.Meta.Features.Days.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(DaysStaticData), fileName = "Days")]
    public class DaysStaticData : BaseStaticData
    {
        public int StartGoldAmount = 50;

        public List<DayData> Days;
    }
}