using System;
using UnityEngine;

namespace Code.Gameplay.StaticData.Data.Formulas
{
    [Serializable]
    public class ExponentGrowthFormula
    {
        public float BaseValue = 1;
        public float StepFactor = 2f;
        public float GrowthExponent = 1.5f;
        public int BonusAdjustment = 1;

        public ExponentGrowthFormula(float baseValue, float growthExponent, float stepFactor, int bonusAdjustment)
        {
            BaseValue = baseValue;
            GrowthExponent = growthExponent;
            StepFactor = stepFactor;
            BonusAdjustment = bonusAdjustment;
        }

        public ExponentGrowthFormula()
        {
            
        }

        public float Calculate(int i)
        {
            return BaseValue + StepFactor * Mathf.Pow(i, GrowthExponent) + BonusAdjustment;
        }
    }

    public static class FormulaExtensions
    {
        public static float Calculate(this ExponentGrowthFormula formula, int i)
        {
            return formula.Calculate(i);
        }
    }
}