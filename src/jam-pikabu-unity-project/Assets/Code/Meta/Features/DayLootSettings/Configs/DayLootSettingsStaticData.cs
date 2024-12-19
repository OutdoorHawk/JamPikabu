using Code.Gameplay.StaticData.Data;
using UnityEngine;

namespace Code.Meta.Features.DayLootSettings.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(DayLootSettingsStaticData), fileName = "IngredientPool")]
    public class DayLootSettingsStaticData : BaseStaticData<DayLootSettingsData>
    {
        public override void OnConfigInit()
        {
            base.OnConfigInit();
            AddIndex(data => data.Id);
        }

        public DayLootSettingsData GetSettingsById(int settingsId)
        {
            return GetByKey(settingsId) ?? Configs[^1];
        }

        public DayLootSettingsData GetDayLootByDayId(int dayIndex)
        {
            foreach (var settings in Configs)
            {
                if (dayIndex < settings.DaysRange.x)
                    continue;
                
                if (dayIndex > settings.DaysRange.y)
                    continue;
                
                return settings;
            }

            return Configs[^1];
        }
    }
}