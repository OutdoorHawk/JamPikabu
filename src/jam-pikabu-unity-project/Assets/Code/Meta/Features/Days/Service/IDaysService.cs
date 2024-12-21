using System;
using System.Collections.Generic;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Configs.Stars;

namespace Code.Meta.Features.Days.Service
{
    public interface IDaysService
    {
        event Action OnEnterRoundPreparation;
        event Action OnDayBegin;
        event Action OnDayComplete;
        List<DayStarData> DayStarsData { get; }
        int CurrentDay { get; }
        int MaxDays { get; }
        void InitializeDays(IEnumerable<DayProgressData> daysProgress);
        bool IsCompletedFirstLevel();
        void SetActiveDay(int selectedDayId);
        List<DayProgressData> GetDaysProgress();
        bool TryGetDayProgress(int dayId, out DayProgressData dayProgress);
        int GetStarsEarnedForDay(int day);
        int GetAllEarnedStars();
        void BeginDay();
        void RoundEnd();
        void EnterRoundPreparation();
        void DayComplete();
        float GetRoundDuration();
        bool CheckAllDaysComplete();
        bool CheckDayUnlocked(int dayId);
        DayData GetDayData();
        DayData GetDayData(int currentDay);
    }
}