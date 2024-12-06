using Code.Gameplay.Features.Currency.Factory;
using Entitas;

namespace Code.Gameplay.Features.Currency.Systems
{
    public class InitGameplayCurrency : IInitializeSystem
    {
        private readonly ICurrencyFactory _currencyFactory;

        public InitGameplayCurrency(ICurrencyFactory currencyFactory)
        {
            _currencyFactory = currencyFactory;
        }

        public void Initialize()
        {
            _currencyFactory.CreateCurrencyStorages();
        }
    }
}