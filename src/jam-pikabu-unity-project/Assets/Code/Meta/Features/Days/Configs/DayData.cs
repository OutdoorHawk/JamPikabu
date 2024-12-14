using System;
using Code.Infrastructure.SceneLoading;

namespace Code.Meta.Features.Days.Configs
{
    [Serializable]
    public class DayData
    {
        public int Day;
        public int PlayCost;
        public int OrdersAmount;
        public float RoundDuration= 30;
        public bool IsBoss;
        public SceneTypeId SceneId = SceneTypeId.Level_1;
    }
}