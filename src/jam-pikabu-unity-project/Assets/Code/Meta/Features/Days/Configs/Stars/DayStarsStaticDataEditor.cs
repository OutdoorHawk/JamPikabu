using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Features.Orders.Config;
using Code.Meta.Features.DayLootSettings.Configs;
using Code.Meta.Features.LootCollection.Configs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Meta.Features.Days.Configs.Stars
{
    public partial class DayStarsStaticData
    {
        [FoldoutGroup("Editor")] public DaysStaticData DaysStaticData;
        [FoldoutGroup("Editor")] public LootProgressionStaticData LootProgression;
        [FoldoutGroup("Editor")] public LootSettingsStaticData LootSettings;
        [FoldoutGroup("Editor")] public DayLootSettingsStaticData DayLootSettings;
        [FoldoutGroup("Editor")] public OrdersStaticData OrdersData;
        
        [FoldoutGroup("Editor")]
        [Button]
        private void CreateConfigsForEveryDay()
        {
            foreach (DayData dayData in DaysStaticData.Configs)
            {
                Configs.Add(new DayStarsSetup());
            }
        }

        [FoldoutGroup("Editor")]
        [Button]
        private void CalculateAverageRatingPerDay()
        {
            bool IgnoreBadIngredients = true;
            foreach (DayData dayData in DaysStaticData.Configs)
            {
                float averageMinRatingPerDay = 0;
                float averageMaxRatingPerDay = 0;

                DayLootSettingsData dayLoot = DayLootSettings.GetDayLootByDayId(dayData.Id);
                List<LootTypeId> availableProducts = dayLoot.AvailableIngredients;
                int ordersPerDay = dayData.OrdersAmount;
                DayLootSettings.OnConfigInit();
                
                for (int i = 0; i < ordersPerDay; i++)
                {
                    List<OrderData> ordersDataConfigs = FindOrdersForDay(dayData);

                    OrderData randomOrder = ordersDataConfigs[Random.Range(0, ordersDataConfigs.Count)];
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
                    LootSetup lootSetup = LootSettings.Configs.Find(data => data.Type == product && data.BaseRatingValue > 0);
                    int minRatingPerProduct = progression.Levels[0].RatingBoostAmount + lootSetup.BaseRatingValue;
                    int maxRatingPerProduct = progression.Levels[^1].RatingBoostAmount + lootSetup.BaseRatingValue;

                    // Factor in the rating contribution for the available products
                    averageMinRatingPerDay += minRatingPerProduct;
                    averageMaxRatingPerDay += maxRatingPerProduct;
                }

                OnConfigInit();
                DayStarsSetup starsSetup = GetDayStarsData(dayData.Id);
                starsSetup.AverageMinRatingPerDay = averageMinRatingPerDay;
                starsSetup.AverageMaxRatingPerDay = averageMaxRatingPerDay;
            }
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

                if (config.Setup.Tag != OrderTag.None)
                {
                    if (DayLootSettings.GetSettingsById(dayData.Id).AvailableOrderTags.Contains(config.Setup.Tag) == false)
                        continue;
                }

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