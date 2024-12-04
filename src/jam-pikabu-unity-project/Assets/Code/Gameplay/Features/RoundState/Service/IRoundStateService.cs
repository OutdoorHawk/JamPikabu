namespace Code.Gameplay.Features.RoundState.Service
{
    public interface IRoundStateService
    {
        int CurrentRound { get; }
        void CreateRoundStateController();
        void RoundComplete();
        void ResetCurrentRound();
        void TryLoadNextLevel();
        void GameOver();
    }
}