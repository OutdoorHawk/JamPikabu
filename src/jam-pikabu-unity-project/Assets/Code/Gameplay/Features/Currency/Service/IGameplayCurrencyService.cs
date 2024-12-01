using System;

namespace Code.Gameplay.Features.Currency.Service
{
    public interface IGameplayCurrencyService
    {
        event Action CurrencyChanged;
        int CurrentGoldCurrency { get; }
        void UpdateCurrentAmount(int newAmount);
        void Cleanup();
    }
}