using System;
using System.Collections.Generic;
using Code.Infrastructure.SceneLoading;
using Code.Meta.Features.BonusLevel.Config;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Configs.Stars;

namespace Code.Meta.Features.Days.Service
{
    public interface IDaysService
    {
        event Action OnEnterRoundPreparation;
        event Action OnDayBegin;
        event Action OnDayComplete;
        BonusLevelType BonusLevelType { get; }
        List<DayStarData> DayStarsData { get; }
        int CurrentDay { get; }
        int MaxDays { get; }
        void SetBonusLevel(BonusLevelData type, SceneTypeId sceneTypeId);
        void InitializeDays(IEnumerable<DayProgressData> daysProgress);
        bool IsCompletedFirstLevel();
        void SetActiveDay(DayData selectedDayId);
        List<DayProgressData> GetDaysProgress();
        bool TryGetDayProgress(int dayId, out DayProgressData dayProgress);
        int GetStarsEarnedForDay(int day);
        int GetAllEarnedStars();
        void BeginDay();
        void RoundEnd();
        void EnterRoundPreparation();
        void StarsRecieved(int starsReceived);
        void DayComplete();
        float GetRoundDuration();
        bool CheckDayUnlocked(int dayId);
        DayData GetDayData();
        DayData GetDayData(int currentDay);
        DayStarsSetup GetDayStarData();
        DayStarsSetup GetDayStarData(int currentDay);
        int GetDayGoldFactor();
    }
}