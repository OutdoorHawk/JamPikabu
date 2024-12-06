using System;

namespace Code.Gameplay.Features.Currency.Service
{
    public interface IGameplayCurrencyService
    {
        event Action CurrencyChanged;
        int CurrentTurnCostGold { get; }
        int GetCurrencyOfType(CurrencyTypeId typeId);
        void UpdateCurrencyAmount(int newAmount, CurrencyTypeId typeId);
        void AddWithdraw(int amount, CurrencyTypeId typeId);
        void UpdateCurrentTurnCostAmount(int newAmount);
        void Cleanup();
    }
}