using System;
using Code.Gameplay.Features.Currency.Behaviours;

namespace Code.Gameplay.Features.Currency.Service
{
    public interface IGameplayCurrencyService
    {
        event Action CurrencyChanged;
        CurrencyHolder Holder { get; }
        void OnConfigsInitInitComplete();
        void RegisterHolder(CurrencyHolder currencyHolder);
        void UnregisterHolder(CurrencyHolder currencyHolder);
        int GetCurrencyOfType(CurrencyTypeId typeId);
        void UpdateCurrencyAmount(int newAmount, int withdraw, CurrencyTypeId typeId);
        void Cleanup();
        void InitCurrency();
    }
}