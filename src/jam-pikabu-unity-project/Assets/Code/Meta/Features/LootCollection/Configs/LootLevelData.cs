using System;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Config;

namespace Code.Meta.Features.LootCollection.Configs
{
    [Serializable]
    public class LootLevelData
    {
        public int RatingBoostAmount = 1;
        public CostSetup Cost = new(CurrencyTypeId.Gold);
    }
}