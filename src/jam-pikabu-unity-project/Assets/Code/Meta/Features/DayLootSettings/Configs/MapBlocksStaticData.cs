using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData.Data;
using UnityEngine;

namespace Code.Meta.Features.DayLootSettings.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(MapBlocksStaticData), fileName = "MapBlocks")]
    public partial class MapBlocksStaticData : BaseStaticData<MapBlockData>
    {
        public const int DAYS_IN_BLOCK = 3;

        private readonly Dictionary<LootTypeId, MapBlockData> _linkedIngredientIndex = new();

        public override void OnConfigInit()
        {
            base.OnConfigInit();
            AddIndex(data => (int)data.UnlocksIngredient);

            _linkedIngredientIndex.Clear();
            foreach (MapBlockData mapBlockData in Configs)
                _linkedIngredientIndex.TryAdd(mapBlockData.UnlocksIngredient, mapBlockData);
        }

        public MapBlockData GetMapBlockDataByLinkedIngredient(LootTypeId lootTypeId)
        {
            return _linkedIngredientIndex.GetValueOrDefault(lootTypeId);
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