using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Distraction;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.StaticData.Data;
using Code.Infrastructure.SceneLoading;
using Sirenix.OdinInspector;

namespace Code.Meta.Features.Days.Configs
{
    [Serializable]
    public class DayData : BaseData
    {
        public bool IsBossDay;
        public OrderTag AvailableOrderTags;
        [FoldoutGroup("Data")] public int OrdersAmount = 3;
        [FoldoutGroup("Data")] public float DayGoldFactor = 1;
        [FoldoutGroup("Data")] public SceneTypeId SceneId = SceneTypeId.DefaultGameplayScene;
        [FoldoutGroup("Data")] public List<DistractionObjectTypeId> DistractionObjects;
    }
}