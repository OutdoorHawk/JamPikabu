using System;

namespace Code.Gameplay.Features.Currency.Config
{
    [Serializable]
    public class CostSetup
    {
        public CurrencyTypeId CurrencyType;
        public int Amount;

        public CostSetup(CurrencyTypeId type, int amount)
        {
            CurrencyType = type;
            Amount = amount;
        }
        
        public CostSetup(CurrencyTypeId type)
        {
            CurrencyType = type;
        }

        public CostSetup(int amount)
        {
            Amount = amount;
        }

        public CostSetup()
        {
            
        }
    }
}