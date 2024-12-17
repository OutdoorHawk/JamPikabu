#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Orders.Config;
using Code.Meta.Features.LootCollection.Configs;
using RoyalGold.Sources.Scripts.Game.MVC.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Meta.Features.Days.Configs
{
    public partial class DaysStaticData
    {
        [FoldoutGroup("Editor")] public OrdersStaticData OrdersData;
        [FoldoutGroup("Editor")] public LootProgressionStaticData LootProgression;

        [FoldoutGroup("Editor")] [ReadOnly] public List<int> AverageGoldPerLevel;
        [FoldoutGroup("Editor")] [ReadOnly] public int TotalGoldPerLevels;

        [FoldoutGroup("Editor")] public float MinGoldFactor = 1f; 
        [FoldoutGroup("Editor")] public float MaxGoldFactor = 5f;
        [FoldoutGroup("Editor")] public AnimationCurve GoldFactorCurve;

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

        [FoldoutGroup("Editor")]
        [Button]
        private void ApplyGoldFactorBalance()
        {
            List<DayData> dayDataConfigs = Configs;

            int totalCount = dayDataConfigs.Count;

            for (int i = 0; i < totalCount; i++)
            {
                float t = i / (float)(totalCount - 1);
                float curveValue = GoldFactorCurve.Evaluate(t);
                float newGoldFactor = Mathf.Lerp(MinGoldFactor, MaxGoldFactor, curveValue);
                
                dayDataConfigs[i].DayGoldFactor = newGoldFactor;
            }
        }

        [Button]
        private void CalculateAverageRatingPerDay()
        {
            foreach (DayData dayData in Configs)
            {
                float averageMinRatingPerDay = 0;
                float averageMaxRatingPerDay = 0;
                
                List<LootTypeId> availableProducts = dayData.AvailableIngredients;
                int ordersPerDay = dayData.OrdersAmount;

                for (int i = 0; i < ordersPerDay; i++)
                {
                    OrderData randomOrder = OrdersData.Configs[Random.Range(0, OrdersData.Configs.Count)];
                    OrderSetup orderSetup = randomOrder.Setup;
                    float averageProductCount = GetAverage(orderSetup.MinMaxNeedAmount);
                    float averageGoodIngredients = GetAverage(orderSetup.MinMaxGoodIngredients);
                    float averageBadIngredients = GetAverage(orderSetup.MinMaxBadIngredients);
                    float averageIngredientFactor = GetAverage(orderSetup.MinMaxIngredientsRatingFactor);
                }
                
                foreach (LootTypeId product in availableProducts)
                {
                    LootProgressionData progression = LootProgression.GetConfig(product);
                    int minRatingPerProduct = progression.Levels[0].RatingBoostAmount;
                    int maxRatingPerProduct = progression.Levels[^1].RatingBoostAmount;
                }
                
                dayData.AverageMinRatingPerDay = averageMinRatingPerDay;
                dayData.AverageMaxRatingPerDay = averageMaxRatingPerDay;
            }
        }

        private static float GetAverage(Vector2Int range)
        {
            return (range.x + range.y) / 2f;
        }
    }
}
#endif