namespace Code.Gameplay.Features.Currency.Handler
{
    public interface IGameplayCurrencyChangedHandler
    {
        void OnCurrencyChanged(CurrencyTypeId type, int newAmount);
    }
}