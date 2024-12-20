using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.StaticData.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Meta.Features.DayLootSettings.Configs
{
    [Serializable]
    public class DayLootSettingsData : BaseData
    {
        [MinMaxSlider(1, 99, showFields: true)] public Vector2Int DaysRange = Vector2Int.one;
        public List<LootTypeId> AvailableIngredients = new();
        public List<OrderTag> AvailableOrderTags = new();
    }
}