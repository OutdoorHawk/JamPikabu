namespace Code.Gameplay.Features.RoundState.Service
{
    public interface IRoundStateService
    {
        int CurrentDay { get; }
        void BeginDay();
        void RoundEnd();
        void PrepareToNextRound();
        void DayComplete();
    }
}