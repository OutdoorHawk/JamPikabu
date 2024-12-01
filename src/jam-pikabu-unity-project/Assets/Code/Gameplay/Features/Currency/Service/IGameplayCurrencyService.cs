using System;

namespace Code.Gameplay.Features.Currency.Service
{
    public interface IGameplayCurrencyService
    {
        event Action CurrencyChanged;
        CurrencyTypeId GoldCurrencyType { get; }
        int CurrentGoldCurrency { get; }
        int CurrentTurnCostGold { get; }
        void UpdateCurrentGoldAmount(int newAmount);
        void UpdateCurrentTurnCostAmount(int newAmount);
        void Cleanup();
    }
}