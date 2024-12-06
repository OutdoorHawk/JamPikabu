namespace Code.Gameplay.Features.RoundState.Service
{
    public interface IRoundStateService
    {
        int CurrentDay { get; }
        void CreateRoundStateController();
        void RoundEnd();
        void PrepareToNextRound();
        void DayComplete();
        void GameOver();
    }
}