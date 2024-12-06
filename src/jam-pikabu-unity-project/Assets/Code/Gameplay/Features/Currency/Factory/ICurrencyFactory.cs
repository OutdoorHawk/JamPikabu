using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;

namespace Code.Gameplay.Features.Currency.Factory
{
    public interface ICurrencyFactory
    {
        void CreateCurrencyStorages();
        void PlayCurrencyAnimation(in CurrencyAnimationParameters parameters);
    }
}