using System.Collections.Generic;
using Code.Gameplay.StaticData.Data;
using UnityEngine;

namespace Code.Meta.Features.Days.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(DaysStaticData), fileName = "Days")]
    public class DaysStaticData : BaseStaticData<DayData>
    {
        public int StartGoldAmount = 0;

        public List<DayData> Days;

        public override void OnConfigInit()
        {
            base.OnConfigInit();
            AddIndex(data => data.Id);
        }

        public DayData GetDayData(int dayIndex)
        {
            return GetByKey(dayIndex);
        }
    }
}