using System.Collections.Generic;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.StaticData.Data.Formulas;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Meta.Features.DayLootSettings.Configs
{
    public partial class MapBlocksStaticData
    {
        [TabGroup("Editor")] public ExponentGrowthFormula StarsFormula;

        [TabGroup("Editor")]
        [Button]
        private void SetupStarsNeedToUnlock()
        {
            int cumulativeStars = 0; // Накопленные звезды от предыдущих блоков

            for (int i = 0; i < Configs.Count; i++)
            {
                MapBlockData mapBlockData = Configs[i];

                if (i == 0)
                {
                    // Для первого блока всегда 0 звезд
                    mapBlockData.StarsNeedToUnlock = 0;
                }
                else
                {
                    // Расчет требуемого количества звезд для текущего блока
                    int maxPreviousStars = cumulativeStars; // Максимум звезд, которые можно было заработать до текущего блока
                    int requiredStars = Mathf.Clamp((int)StarsFormula.Calculate(i), 0, maxPreviousStars);

                    // Устанавливаем звезды для разблокировки текущего блока
                    mapBlockData.StarsNeedToUnlock = requiredStars;
                }

                // Обновляем накопленное количество звезд
                cumulativeStars += DAYS_IN_BLOCK * 3; // Максимум звезд за текущий блок
            }
        }

        [TabGroup("Editor")]
        [Button]
        private void SetupTimeToFreeUpgrade()
        {
            for (int i = 0; i < Configs.Count; i++)
            {
                Configs[i].FreeUpgradeTimeMinutes = Configs[0].FreeUpgradeTimeMinutes + 10 * i;
            }
        }
        
        [TabGroup("Editor")]
        [Button]
        private void SetupAvailableIngredients()
        {
            int availableIngredientsIndex = 0; // Index to cycle through AvailableIngredients

            for (int i = 2; i < Configs.Count; i++)
            {
                MapBlockData mapBlockData = Configs[i];

                MapBlockData previousBlockData = Configs[i - 1];
                
                if (i > 0)
                {
                    mapBlockData.AvailableIngredients = new List<LootTypeId>(previousBlockData.AvailableIngredients);
                }
                else if (mapBlockData.AvailableIngredients.Count == 0)
                {
                    mapBlockData.AvailableIngredients.Add(mapBlockData.UnlocksIngredient);
                }

                if (mapBlockData.AvailableIngredients.Count > 0)
                {
                    // Replace the ingredient at the current index
                    mapBlockData.AvailableIngredients[availableIngredientsIndex] = mapBlockData.UnlocksIngredient;

                    // Increment the index and reset to 0 if it exceeds the list size
                    availableIngredientsIndex = (availableIngredientsIndex + 1) % mapBlockData.AvailableIngredients.Count;
                }
            }
        }
        
        private void SetupUnlocks()
        {
            int j = 1;
            for (var i = 1; i < Configs.Count; i++)
            {
                if ((LootTypeId)j is LootTypeId.GoldCoin or LootTypeId.Wood or LootTypeId.Spoon or LootTypeId.Sock)
                    continue;
                
                Configs[i].UnlocksIngredient = (LootTypeId)j;
                i++;
                j++;
            }
        }
    }
}