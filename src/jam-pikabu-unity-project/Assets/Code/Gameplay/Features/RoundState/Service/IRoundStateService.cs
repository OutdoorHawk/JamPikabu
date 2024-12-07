using System;
using Code.Gameplay.Features.RoundState.Configs;

namespace Code.Gameplay.Features.RoundState.Service
{
    public interface IRoundStateService
    {
        event Action OnEnterRoundPreparation;
        event Action OnDayBegin;
        event Action OnDayComplete;
        int CurrentDay { get; }
        int MaxDays { get; }
        void BeginDay();
        void RoundEnd();
        void PrepareToNextRound();
        void DayComplete();
        DayData GetDayData();
        void GameOver();
    }
}