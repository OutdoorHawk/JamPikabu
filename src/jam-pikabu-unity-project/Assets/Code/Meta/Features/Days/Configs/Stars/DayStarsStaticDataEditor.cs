#if UNITY_EDITOR
using System;
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
        [TabGroup("Editor")] public DaysStaticData DaysStaticData;
        [TabGroup("Editor")] public LootProgressionStaticData LootProgression;
        [TabGroup("Editor")] public LootSettingsStaticData LootSettings;
        [TabGroup("Editor")] public MapBlocksStaticData DayLootSettings;
        [TabGroup("Editor")] public OrdersStaticData OrdersData;
        
        [TabGroup("Editor")] public float StepFactor = 2f;
        [TabGroup("Editor")] public int StartRatingDayFormula = 3;
        
        [TabGroup("Editor")]
        [Button]
        private void CreateLevelsAndApplyNeedStarsFormula()
        {
            for (int i = 0; i < DaysStaticData.Configs.Count; i++)
            {
                if (i < StartRatingDayFormula - 1)
                    continue;

                int ratingAmountNeed = (int)RoundToNearestFive(Configs[i - 1].RatingNeedAll + StepFactor);

                if (DaysStaticData.Configs[i].IsBossDay) 
                    ratingAmountNeed = (int)RoundToNearestFive(ratingAmountNeed / 1.5f);

                if (i >= Configs.Count)
                {
                    var dayData = new DayStarsSetup()
                    {
                        RatingNeedAll = ratingAmountNeed,
                    };

                    Configs.Add(dayData);
                    continue;
                }

                Configs[i].RatingNeedAll = Math.Min(ratingAmountNeed, 150);
            }
        }

        private float RoundToNearestFive(float value)
        {
            return Mathf.Round(value / 5) * 5;
        }

        [TabGroup("Editor")]
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
                OrdersData.OnConfigInit();
                DayLootSettings.OnConfigInit();

                for (int i = 0; i < ordersPerDay; i++)
                {
                    OrderSetup orderSetup = GetRandomOrderSetup(dayData);
                    availableProducts.ShuffleList();
                    
                    int maxEachLootCount = orderSetup.MinMaxNeedAmount.y;
                    int goodIngredientsCount = orderSetup.MinMaxGoodIngredients.y;
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
            List<OrderData> ordersDataConfigs = OrdersData.GetOrdersByTag(dayData.AvailableOrderTags);
            OrderData randomOrder = ordersDataConfigs[Random.Range(0, ordersDataConfigs.Count)];
            OrderSetup orderSetup = randomOrder.Setup;
            return orderSetup;
        }

        private (int minRatingPerProduct, int MaxRatingPerProduct) GetProductRating(LootTypeId product)
        {
            LootProgressionData progression = LootProgression.Configs.Find(data => data.Type == product);
            LootSettingsData lootSetup = LootSettings.Configs.Find(data => data.Type == product && data.CanBeUsedInOrders);
            int minRatingPerProduct = progression.Levels[0].RatingBoostAmount + lootSetup.BaseRatingValue;
            int maxRatingPerProduct = progression.Levels[^1].RatingBoostAmount + lootSetup.BaseRatingValue;
            return (minRatingPerProduct, maxRatingPerProduct);
        }

        private List<OrderData> FindOrdersForDay(DayData dayData)
        {
            List<OrderData> ordersDataConfigs = new List<OrderData>();
            foreach (var config in OrdersData.Configs)
            {
                if (config.Setup.DaysRange.y > 0 && dayData.Id > config.Setup.DaysRange.y)
                    continue;

                if (config.Setup.DaysRange.x > 0 && dayData.Id < config.Setup.DaysRange.x)
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