using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;

namespace Code.Gameplay.Features.Currency.Factory
{
    public interface ICurrencyFactory
    {
        void CreateCurrencyStorages(int goldGold);
        void PlayCurrencyAnimation(in CurrencyAnimationParameters parameters);
        void CreateAddCurrencyRequest(CurrencyTypeId type, int amount, int withdraw = 0);
    }
}