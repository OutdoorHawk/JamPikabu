using System;
using Sirenix.OdinInspector;

namespace Code.Gameplay.Features.Currency.Config
{
    [Serializable]
    public class CurrencyConfig
    {
        public CurrencyTypeId CurrencyTypeId;
        public CurrencySetup Data;
    }
}