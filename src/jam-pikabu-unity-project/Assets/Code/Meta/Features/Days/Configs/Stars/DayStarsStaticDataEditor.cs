#if UNITY_EDITOR
using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Orders.Config;
using Code.Meta.Features.DayLootSettings.Configs;
using Code.Meta.Features.LootCollection.Configs;
using RoyalGold.Sources.Scripts.Game.MVC.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Meta.Features.Days.Configs.Stars
{
    public partial class DayStarsStaticData
    {
        [FoldoutGroup("Editor")] public DaysStaticData DaysStaticData;
        [FoldoutGroup("Editor")] public LootProgressionStaticData LootProgression;
        [FoldoutGroup("Editor")] public LootSettingsStaticData LootSettings;
        [FoldoutGroup("Editor")] public MapBlocksStaticData DayLootSettings;
        [FoldoutGroup("Editor")] public OrdersStaticData OrdersData;

        [FoldoutGroup("Editor")] public float BaseRatingNeedAmount = 1;
        [FoldoutGroup("Editor")] public float GrowthExponent = 1.5f;
        [FoldoutGroup("Editor")] public float StepFactor = 2f;
        [FoldoutGroup("Editor")] public int BonusAdjustment = 1;

        [FoldoutGroup("Editor")]
        [Button]
        private void CreateLevelsAndApplyNeedStarsFormula()
        {
            for (int i = 0; i < DaysStaticData.Configs.Count; i++)
            {
                int ratingAmountNeed = (int)RoundToNearestFive(BaseRatingNeedAmount + StepFactor * Mathf.Pow(i, GrowthExponent) + BonusAdjustment);

                if (i >= Configs.Count)
                {
                    var dayData = new DayStarsSetup()
                    {
                        RatingNeedAll = ratingAmountNeed,
                    };

                    Configs.Add(dayData);
                    continue;
                }

                Configs[i].RatingNeedAll = ratingAmountNeed;
            }
        }

        private float RoundToNearestFive(float value)
        {
            return Mathf.Round(value / 5) * 5;
        }

        [FoldoutGroup("Editor")]
        [Button]
        private void CalculateAverageRatingPerDay()
        {
            foreach (DayData dayData in DaysStaticData.Configs)
            {
                float averageMinRatingPerDay = 0;
                float averageMaxRatingPerDay = 0;

                MapBlockData dayLoot = DayLootSettings.GetMapBlockDataByDayId(dayData.Id);
                List<LootTypeId> availableProducts = dayLoot.AvailableIngredients;
                int ordersPerDay = dayData.OrdersAmount;
                DayLootSettings.OnConfigInit();

                for (int i = 0; i < ordersPerDay; i++)
                {
                    OrderSetup orderSetup = GetRandomOrderSetup(dayData);
                    availableProducts.ShuffleList();
                    
                    int maxEachLootCount = orderSetup.MinMaxNeedAmount.y;
                    int goodIngredientsCount = Mathf.Min(availableProducts.Count, orderSetup.MinMaxGoodIngredients.y) ;
                    int ingredientFactorMax = orderSetup.MinMaxIngredientsRatingFactor.y;

                    for (int j = 0; j < goodIngredientsCount; j++)
                    {
                        (int minRatingPerProduct, int maxRatingPerProduct) = GetProductRating(availableProducts[j]);
                        
                        averageMinRatingPerDay += minRatingPerProduct * maxEachLootCount * ingredientFactorMax;
                        averageMaxRatingPerDay += maxRatingPerProduct * maxEachLootCount * ingredientFactorMax;
                    }
                }
                
                OnConfigInit();
                DayStarsSetup starsSetup = GetDayStarsData(dayData.Id);
                starsSetup.AverageMinRatingPerDay = averageMinRatingPerDay;
                starsSetup.AverageMaxRatingPerDay = averageMaxRatingPerDay;
            }
        }

        private OrderSetup GetRandomOrderSetup(DayData dayData)
        {
            List<OrderData> ordersDataConfigs = FindOrdersForDay(dayData);
            OrderData randomOrder = ordersDataConfigs[Random.Range(0, ordersDataConfigs.Count)];
            OrderSetup orderSetup = randomOrder.Setup;
            return orderSetup;
        }

        private (int minRatingPerProduct, int MaxRatingPerProduct) GetProductRating(LootTypeId product)
        {
            LootProgressionData progression = LootProgression.Configs.Find(data => data.Type == product);
            LootSetup lootSetup = LootSettings.Configs.Find(data => data.Type == product && data.CanBeUsedInOrders);
            int minRatingPerProduct = progression.Levels[0].RatingBoostAmount + lootSetup.BaseRatingValue;
            int maxRatingPerProduct = progression.Levels[^1].RatingBoostAmount + lootSetup.BaseRatingValue;
            return (minRatingPerProduct, maxRatingPerProduct);
        }

        private List<OrderData> FindOrdersForDay(DayData dayData)
        {
            List<OrderData> ordersDataConfigs = new List<OrderData>();
            foreach (var config in OrdersData.Configs)
            {
                if (config.Setup.MinMaxDayToUnlock.y > 0 && dayData.Id > config.Setup.MinMaxDayToUnlock.y)
                    continue;

                if (config.Setup.MinMaxDayToUnlock.x > 0 && dayData.Id < config.Setup.MinMaxDayToUnlock.x)
                    continue;

                if (dayData.AvailableOrderTags is not OrderTag.None && dayData.AvailableOrderTags.HasFlag(config.Setup.Tag) == false)
                    continue;

                ordersDataConfigs.Add(config);
            }

            return ordersDataConfigs;
        }

        private static float GetAverage(Vector2Int range)
        {
            return (range.x + range.y) / 2f;
        }
    }
}
#endif