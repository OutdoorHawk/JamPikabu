namespace Code.Gameplay.Features.RoundState.Service
{
    public interface IRoundStateService
    {
        int CurrentRound { get; }
        int CurrentDay { get; }
        void CreateRoundStateController();
        void RoundComplete();
        void PrepareToNextRound();
        void DayComplete();
        void TryLoadNextLevel();
        void GameOver();
    }
}