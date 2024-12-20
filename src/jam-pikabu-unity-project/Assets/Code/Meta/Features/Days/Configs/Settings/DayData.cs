using System;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData.Data;
using Code.Infrastructure.SceneLoading;
using Sirenix.OdinInspector;

namespace Code.Meta.Features.Days.Configs
{
    [Serializable]
    public class DayData : BaseData
    {
        public LootTypeId UnlocksIngredient;
        public bool IsBossDay;
        [FoldoutGroup("Data")] public int OrdersAmount = 3;
        [FoldoutGroup("Data")] public float DayGoldFactor = 1;
        [FoldoutGroup("Data")] public SceneTypeId SceneId = SceneTypeId.Level_1;
    }
}