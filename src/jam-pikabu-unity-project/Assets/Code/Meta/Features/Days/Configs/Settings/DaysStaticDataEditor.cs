#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Code.Gameplay.Features.Orders.Config;
using Code.Gameplay.StaticData.Data.Formulas;
using RoyalGold.Sources.Scripts.Game.MVC.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Meta.Features.Days.Configs
{
    public partial class DaysStaticData
    {
        [TabGroup("Editor")] public OrdersStaticData OrdersData;

        [TabGroup("Editor")] [ReadOnly] public List<int> AverageGoldPerLevel;
        [TabGroup("Editor")] [ReadOnly] public int TotalGoldPerLevels;

        [TabGroup("Editor")] [ReadOnly] public bool IgnoreBadIngredients;

        [TabGroup("Editor")] public int LevelsAmount = 18;
        [TabGroup("Editor")] public ExponentGrowthFormula GoldFormula = new(1, 0.62f, 0.11f, 0);
        [TabGroup("Editor")] public List<DayOrderGroupEditor> OrdersRange;

        [TabGroup("Editor")]
        [Button]
        private void CreateLevelsAndApplyFormula()
        {
            for (int i = 1; i < LevelsAmount; i++)
            {
                float goldFactor = GoldFormula.Calculate(i);

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

        [TabGroup("Editor")]
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

        [TabGroup("Editor")]
        [Button]
        private void SetupOrdersRange()
        {
            foreach (DayData dayData in Configs)
            {
                DayOrderGroupEditor group = OrdersRange.Find(order => order.CanUse(dayData.Id));
                dayData.AvailableOrderTags = group.OrderTag;
                if (dayData.IsBossDay) 
                    dayData.AvailableOrderTags |= OrderTag.Boss;
            }
        }
    }
}
#endif