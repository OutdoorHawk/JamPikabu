using System;
using Code.Meta.Features.Days.Configs;

namespace Code.Meta.Features.Days.Service
{
    public interface IDaysService
    {
        event Action OnEnterRoundPreparation;
        event Action OnDayBegin;
        event Action OnDayComplete;
        int CurrentDay { get; }
        int MaxDays { get; }
        void SetCurrentDay(int day);
        void BeginDay();
        void RoundEnd();
        void EnterRoundPreparation();
        void DayComplete();
        bool CheckAllDaysComplete();
        DayData GetDayData();
        DayData GetDayData(int currentDay);
    }
}