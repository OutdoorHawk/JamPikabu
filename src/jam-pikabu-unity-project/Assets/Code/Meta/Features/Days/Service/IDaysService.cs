using System;
using System.Collections.Generic;
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
        void InitializeDays(IEnumerable<DayProgressData> daysProgress);
        bool IsCompletedFirstLevel();
        void SetActiveDay(int selectedDayId);
        List<DayProgressData> GetDaysProgress();
        bool TryGetDayProgress(int dayId, out DayProgressData dayProgress);
        int GetStarsEarnedForDay(int day);
        void BeginDay();
        void RoundEnd();
        void EnterRoundPreparation();
        void DayComplete();
        bool CheckAllDaysComplete();
        DayData GetDayData();
        DayData GetDayData(int currentDay);
    }
}