using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using static Code.Meta.Features.DayLootSettings.Configs.MapBlocksStaticData;

namespace Code.Meta.Features.DayLootSettings.Configs
{
    [Serializable]
    public class MapBlockData : BaseData
    {
        [
            PropertyOrder(0),
            ReadOnly,
            ShowInInspector,
            MinMaxSlider(1, 99, showFields: true)
        ]
        public Vector2Int DaysRange => GetDaysRange();

        public LootTypeId UnlocksIngredient;

        public List<LootTypeId> AvailableIngredients = new();

        private Vector2Int GetDaysRange()
        {
            int firstDay = (Id - 1) * DAYS_IN_BLOCK + 1;
            int lastDay = Id * DAYS_IN_BLOCK;
            return new Vector2Int(firstDay, lastDay);
        }
    }
}