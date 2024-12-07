using System;

namespace Code.Gameplay.Features.Currency.Service
{
    public interface IGameplayCurrencyService
    {
        event Action CurrencyChanged;
        int CurrentTurnCostGold { get; }
        void OnConfigsInitInitComplete();
        int GetCurrencyOfType(CurrencyTypeId typeId);
        void UpdateCurrencyAmount(int newAmount, int withdraw, CurrencyTypeId typeId);
        void UpdateCurrentTurnCostAmount(int newAmount);
        void Cleanup();
        void InitCurrency();
    }
}