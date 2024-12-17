#if UNITY_EDITOR
using System.Collections.Generic;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Meta.Features.LootCollection.Configs
{
    public partial class LootProgressionStaticData
    {
        [FoldoutGroup("Editor")] public int LevelsAmount = 15;
        [FoldoutGroup("Editor")] public float GrowthExponent = 1.5f;
        [FoldoutGroup("Editor")] public float StepFactor = 2f;
        [FoldoutGroup("Editor")] public int BonusAdjustment = 1;

        [FoldoutGroup("Editor")] public AnimationCurve ProgressionCurve;

        [Button]
        private void RecreateLevels()
        {
            foreach (var config in Configs)
            {
                config.Levels.Clear();
                for (int i = 0; i < LevelsAmount; i++) 
                    config.Levels.Add(new LootLevelData());
            }
        }

        [Button]
        private void ApplyLootProgressionFormula()
        {
            ProgressionCurve = new AnimationCurve();
            foreach (LootProgressionData lootProgression in Configs)
            {
                List<LootLevelData> levels = lootProgression.Levels;

                int baseBoostAmount = levels[0].RatingBoostAmount;
                int baseCostAmount = levels[0].Cost.Amount;
                CurrencyTypeId currencyType = levels[0].Cost.CurrencyType;

                // Итерация по уровням и применение формулы
                for (int i = 1; i < levels.Count; i++)
                {
                    float boostValue = baseBoostAmount + StepFactor * Mathf.Pow(i, GrowthExponent) + BonusAdjustment;
                    float costValue = baseCostAmount + StepFactor * Mathf.Pow(i, GrowthExponent) + BonusAdjustment;

                    levels[i].RatingBoostAmount = Mathf.RoundToInt(boostValue);
                    levels[i].Cost = new CostSetup(currencyType, Mathf.RoundToInt(costValue));
                    ProgressionCurve.AddKey(i, boostValue);
                }
            }
        }
    }
}
#endif