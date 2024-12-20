#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Code.Gameplay.Features.Orders.Config;
using RoyalGold.Sources.Scripts.Game.MVC.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Meta.Features.Days.Configs
{
    public partial class DaysStaticData
    {
        [FoldoutGroup("Editor")] public OrdersStaticData OrdersData;

        [FoldoutGroup("Editor")] [ReadOnly] public List<int> AverageGoldPerLevel;
        [FoldoutGroup("Editor")] [ReadOnly] public int TotalGoldPerLevels;

        [FoldoutGroup("Editor")] [ReadOnly] public bool IgnoreBadIngredients;

        [FoldoutGroup("Editor")] public int LevelsAmount = 18;
        [FoldoutGroup("Editor")] public float BaseGoldFactor = 1;
        [FoldoutGroup("Editor")] public float GrowthExponent = 1.5f;
        [FoldoutGroup("Editor")] public float StepFactor = 2f;
        [FoldoutGroup("Editor")] public int BonusAdjustment = 1;

        [FoldoutGroup("Editor")]
        [Button]
        private void CreateLevelsAndApplyFormula()
        {
            for (int i = 1; i < LevelsAmount; i++)
            {
                float goldFactor = BaseGoldFactor + StepFactor * Mathf.Pow(i, GrowthExponent) + BonusAdjustment;

                if (i >= Configs.Count)
                {
                    var dayData = new DayData
                    {
                        DayGoldFactor = goldFactor,
                        OrdersAmount = Random.Range(3, 5),
                    };

                    Configs.Add(dayData);
                    continue;
                }

                Configs[i].DayGoldFactor = goldFactor;
            }
        }

        [FoldoutGroup("Editor")]
        [Button]
        public void CalculateAverageGoldPerLevel()
        {
            AverageGoldPerLevel.Clear();
            List<OrderData> orderConfigs = OrdersData.Configs;
            List<DayData> dayDataConfigs = Configs;

            foreach (var day in dayDataConfigs)
            {
                int totalGoldPerDay = 0;

                orderConfigs.ShuffleList();

                for (int i = 0; i < day.OrdersAmount; i++)
                {
                    float goldPerOrder = orderConfigs[i].Setup.GoldReward.Amount * day.DayGoldFactor;
                    totalGoldPerDay += Mathf.RoundToInt(goldPerOrder);
                }

                AverageGoldPerLevel.Add(totalGoldPerDay);
            }

            TotalGoldPerLevels = AverageGoldPerLevel.Sum();
        }
    }
}
#endif