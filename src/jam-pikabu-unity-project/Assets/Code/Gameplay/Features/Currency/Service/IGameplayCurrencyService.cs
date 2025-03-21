using System;
using System.Collections.Generic;
using Code.Gameplay.Features.Currency.Behaviours;

namespace Code.Gameplay.Features.Currency.Service
{
    public interface IGameplayCurrencyService
    {
        event Action CurrencyChanged;
        CurrencyHolder Holder { get; }
        IReadOnlyDictionary<CurrencyTypeId, CurrencyCount> Currencies { get; }
        void RegisterHolder(CurrencyHolder currencyHolder);
        void UnregisterHolder(CurrencyHolder currencyHolder);
        int GetCurrencyOfType(CurrencyTypeId typeId, bool applyWithdraw = true);
        void UpdateCurrencyAmount(int newAmount, int withdraw, CurrencyTypeId typeId);
    }
}