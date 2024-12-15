using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData.Data;
using Code.Infrastructure.SceneLoading;

namespace Code.Meta.Features.Days.Configs
{
    [Serializable]
    public class DayData : BaseData
    {
        public int OrdersAmount;
        public float RoundDuration = 20;
        public bool IsBossDay;
        public SceneTypeId SceneId = SceneTypeId.Level_1;
        public List<LootTypeId> AvailableIngredients;
        public List<DayStarData> Stars = new() { new DayStarData(), new DayStarData(), new DayStarData() };
    }
}