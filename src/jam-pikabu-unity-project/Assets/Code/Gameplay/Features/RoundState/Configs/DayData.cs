using System;
using Code.Infrastructure.SceneLoading;

namespace Code.Gameplay.Features.RoundState.Configs
{
    [Serializable]
    public class DayData
    {
        public int Day;
        public int PlayCost;
        public int OrdersAmount;
        public SceneTypeId SceneId = SceneTypeId.Level_1;
    }
}