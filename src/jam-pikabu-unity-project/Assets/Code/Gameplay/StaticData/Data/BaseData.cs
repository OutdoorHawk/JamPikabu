using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Infrastructure.SceneLoading;
using Code.Meta.Features.Days.Configs;
using Sirenix.OdinInspector;

namespace Code.Gameplay.StaticData.Data
{
    public abstract class BaseData
    {
        public int Id;
        [FoldoutGroup("Data")] public int OrdersAmount;
        [FoldoutGroup("Data")] public float RoundDuration = 20;
        [FoldoutGroup("Data")] public bool IsBossDay;
        [FoldoutGroup("Data")] public SceneTypeId SceneId = SceneTypeId.Level_1;
        [FoldoutGroup("Data")] public List<LootTypeId> AvailableIngredients;
        [FoldoutGroup("Data")] public List<DayStarData> Stars = new() { new DayStarData(), new DayStarData(), new DayStarData() };
    }
}