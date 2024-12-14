using System;

namespace Code.Gameplay.Features.Currency.Service
{
    public interface IGameplayCurrencyService
    {
        event Action CurrencyChanged;
        void OnConfigsInitInitComplete();
        int GetCurrencyOfType(CurrencyTypeId typeId);
        void UpdateCurrencyAmount(int newAmount, int withdraw, CurrencyTypeId typeId);
        void Cleanup();
        void InitCurrency();
    }
}