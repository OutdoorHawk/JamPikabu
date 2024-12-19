using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Meta.Features.DayLootSettings.Configs
{
    [Serializable]
    public class DayLootSettingsData : BaseData
    {
        [MinMaxSlider(1, 99)] public Vector2Int DaysRange;
        public List<LootTypeId> AvailableIngredients = new();
    }
}