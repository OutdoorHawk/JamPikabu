using Code.Gameplay.StaticData.Data;
using UnityEngine;

namespace Code.Meta.Features.DayLootSettings.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(MapBlocksStaticData), fileName = "MapBlocks")]
    public class MapBlocksStaticData : BaseStaticData<MapBlockData>
    {
        public const int DAYS_IN_BLOCK = 3;
        
        public override void OnConfigInit()
        {
            base.OnConfigInit();
            AddIndex(data => data.Id);
        }

        public MapBlockData GetMapBlockDataByMapBlockId(int settingsId)
        {
            return GetByKey(settingsId) ?? Configs[^1];
        }

        public MapBlockData GetMapBlockDataByDayId(int dayId)
        {
            foreach (var settings in Configs)
            {
                int mapBlockFirstDay = settings.DaysRange.x;
                int mapBlockLastDay = settings.DaysRange.y;
               
                if (dayId < mapBlockFirstDay)
                    continue;
                
                if (dayId > mapBlockLastDay)
                    continue;
                
                return settings;
            }

            return Configs[^1];
        }
    }
}