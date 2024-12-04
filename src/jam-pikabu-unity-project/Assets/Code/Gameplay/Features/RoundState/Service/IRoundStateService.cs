namespace Code.Gameplay.Features.RoundState.Service
{
    public interface IRoundStateService
    {
        void CreateRoundStateController();
        void RoundComplete();
        void ResetCurrentRound();
        void TryLoadNextLevel();
    }
}