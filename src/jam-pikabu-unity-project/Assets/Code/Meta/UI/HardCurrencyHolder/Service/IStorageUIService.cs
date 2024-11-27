using System;

namespace Code.Meta.UI.HardCurrencyHolder.Service
{
    public interface IStorageUIService
    {
        event Action HardChanged;
        float CurrentHard { get; }
        void UpdateCurrentHard(float newAmount);
        void Cleanup();
    }
}