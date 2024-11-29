using System;

namespace Code.Gameplay.Features.Currency.Config
{
    [Serializable]
    public class CostSetup
    {
        public CurrencyTypeId CurrencyType;
        public int Amount;

        public CostSetup(CurrencyTypeId type)
        {
            CurrencyType = type;
        }
    }
}