using Code.Gameplay.StaticData.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Meta.Features.Days.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(DaysStaticData), fileName = "Days")]
    public partial class DaysStaticData : BaseStaticData<DayData>
    {
        [TabGroup("Default")] public int StartGoldAmount = 0;
        [TabGroup("Default")] public float DefaultRoundDuration = 25;
        [TabGroup("Default")] public float BossRoundDuration = 45;
        [TabGroup("Default")] public int DefaultRoundHookAttempts = 5;
        [TabGroup("Default")] public int BossRoundHookAttempts = 8;
        [TabGroup("Default")] public int DayToStartTimer = 5;

        public override void OnConfigInit()
        {
            base.OnConfigInit();
            AddIndex(data => data.Id);
        }

        public DayData GetDayData(int dayIndex)
        {
            return GetByKey(dayIndex) ?? Configs[^1];
        }
    }
}