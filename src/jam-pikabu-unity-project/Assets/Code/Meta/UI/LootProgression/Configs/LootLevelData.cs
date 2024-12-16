using System;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;

namespace Code.Meta.UI.LootProgression.Configs
{
    [Serializable]
    public class LootLevelData
    {
        public int RatingBoostAmount;
        public CostSetup Cost = new(CurrencyTypeId.Gold);
    }
}