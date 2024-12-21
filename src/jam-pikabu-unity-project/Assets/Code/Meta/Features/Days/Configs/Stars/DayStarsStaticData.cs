using Code.Gameplay.StaticData.Data;
using UnityEngine;

namespace Code.Meta.Features.Days.Configs.Stars
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(DayStarsStaticData), fileName = "DayStars")]
    public partial class DayStarsStaticData : BaseStaticData<DayStarsSetup>
    {
        public float[] StarsFactorSetup = { 0.1f, 0.5f, 0.9f };

        public override void OnConfigInit()
        {
            base.OnConfigInit();
            AddIndex(data => data.Id);
        }

        public DayStarsSetup GetDayStarsData(int dayIndex)
        {
            return GetByKey(dayIndex) ?? Configs[^1];
        }
    }
}