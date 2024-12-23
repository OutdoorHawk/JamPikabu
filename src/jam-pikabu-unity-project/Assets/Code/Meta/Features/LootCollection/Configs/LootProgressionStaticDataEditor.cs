#if UNITY_EDITOR
using System.Collections.Generic;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.StaticData.Data.Formulas;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Meta.Features.LootCollection.Configs
{
    public partial class LootProgressionStaticData
    {
        [TabGroup("Editor")] public int LevelsAmount = 15;

        [TabGroup("Editor")] public float IncreaseFactor = 1.1f;

        [TabGroup("Editor")] public ExponentGrowthFormula RatingFormula = new(1, 0.8f, 4, 2);
        [TabGroup("Editor")] public ExponentGrowthFormula CostFormula = new(1, 0.8f, 4, 2);

        [TabGroup("Editor"), ReadOnly] public int FullUpgradeCost;
        [TabGroup("Editor")] public AnimationCurve ProgressionCurve;

        [TabGroup("Editor")]
        [Button]
        private void ApplyLootProgressionFormula()
        {
            RecreateLevels();
            ProgressionCurve = new AnimationCurve();
            float increaseFactor = 1;
            foreach (LootProgressionData lootProgression in Configs)
            {
                List<LootLevelData> levels = lootProgression.Levels;
                CurrencyTypeId currencyType = levels[0].Cost.CurrencyType;
                levels[0].Cost.Amount = Mathf.CeilToInt(CostFormula.BaseValue * increaseFactor);

                for (int i = 1; i < levels.Count; i++)
                {
                    float boostValue = RatingFormula.Calculate(i) * increaseFactor;
                    float costValue = CostFormula.Calculate(i) * increaseFactor;

                    levels[i].RatingBoostAmount = Mathf.CeilToInt(boostValue);
                    levels[i].Cost = new CostSetup(currencyType, Mathf.CeilToInt(costValue));
                    ProgressionCurve.AddKey(i, boostValue);
                }

                increaseFactor += IncreaseFactor;
            }

            CalculateFullUpgradeCost();
        }

        private void CalculateFullUpgradeCost()
        {
            FullUpgradeCost = 0;
            foreach (var lootProgression in Configs)
            {
                foreach (var level in lootProgression.Levels)
                {
                    FullUpgradeCost += level.Cost.Amount;
                }
            }
        }

        private void RecreateLevels()
        {
            foreach (var config in Configs)
            {
                config.Levels.Clear();
                for (int i = 0; i < LevelsAmount; i++)
                    config.Levels.Add(new LootLevelData());
                
                config.Levels[0].RatingBoostAmount = 0;
            }
        }
    }
}
#endif