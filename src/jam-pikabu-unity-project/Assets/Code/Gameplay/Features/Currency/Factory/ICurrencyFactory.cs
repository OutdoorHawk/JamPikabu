using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;

namespace Code.Gameplay.Features.Currency.Factory
{
    public interface ICurrencyFactory
    {
        void CreateCurrencyStorages(int goldGold, int plusPlus, int minusMinus);
        void PlayCurrencyAnimation(in CurrencyAnimationParameters parameters);
    }
}