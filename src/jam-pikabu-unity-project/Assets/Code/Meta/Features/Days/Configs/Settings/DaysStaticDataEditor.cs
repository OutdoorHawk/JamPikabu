#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Orders.Config;
using Code.Meta.Features.DayLootSettings.Configs;
using Code.Meta.Features.Days.Configs.Stars;
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
        [FoldoutGroup("Editor")] public DayLootSettingsStaticData DayLootSettings;
        [FoldoutGroup("Editor")] public DayStarsStaticData DayStars;

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

        [FoldoutGroup("Editor")]
        [Button]
        private void CalculateAverageRatingPerDay()
        {
            foreach (DayData dayData in Configs)
            {
                float averageMinRatingPerDay = 0;
                float averageMaxRatingPerDay = 0;

                DayLootSettingsData dayLoot = DayLootSettings.GetDayLootByDayId(dayData.Id);
                List<LootTypeId> availableProducts = dayLoot.AvailableIngredients;
                int ordersPerDay = dayData.OrdersAmount;

                for (int i = 0; i < ordersPerDay; i++)
                {
                    OrderData randomOrder = OrdersData.Configs[Random.Range(0, OrdersData.Configs.Count)];
                    OrderSetup orderSetup = randomOrder.Setup;
                    float averageLootTypesCount = GetAverage(orderSetup.MinMaxNeedAmount);
                    float averageGoodIngredients = GetAverage(orderSetup.MinMaxGoodIngredients);
                    float averageBadIngredients = GetAverage(orderSetup.MinMaxBadIngredients);
                    float averageIngredientFactor = GetAverage(orderSetup.MinMaxIngredientsRatingFactor);

                    // Calculate rating contributions for a single order
                    float minRatingForOrder =
                        (averageGoodIngredients * averageLootTypesCount * averageIngredientFactor) -
                        (IgnoreBadIngredients ? 0 : averageBadIngredients * averageLootTypesCount * averageIngredientFactor);

                    float maxRatingForOrder =
                        (averageGoodIngredients * averageLootTypesCount * averageIngredientFactor) -
                        (IgnoreBadIngredients ? 0 : averageBadIngredients * averageLootTypesCount * averageIngredientFactor);

                    // Accumulate to daily totals
                    averageMinRatingPerDay += minRatingForOrder;
                    averageMaxRatingPerDay += maxRatingForOrder;
                }

                foreach (LootTypeId product in availableProducts)
                {
                    LootProgressionData progression = LootProgression.Configs.Find(data => data.Type == product);
                    int minRatingPerProduct = progression.Levels[0].RatingBoostAmount;
                    int maxRatingPerProduct = progression.Levels[^1].RatingBoostAmount;

                    // Factor in the rating contribution for the available products
                    averageMinRatingPerDay += minRatingPerProduct;
                    averageMaxRatingPerDay += maxRatingPerProduct;
                }

                DayStarsSetup starsSetup = DayStars.GetDayStarsData(dayData.Id);
                starsSetup.AverageMinRatingPerDay = averageMinRatingPerDay;
                starsSetup.AverageMaxRatingPerDay = averageMaxRatingPerDay;
            }
        }

        private static float GetAverage(Vector2Int range)
        {
            return (range.x + range.y) / 2f;
        }
    }
}
#endif