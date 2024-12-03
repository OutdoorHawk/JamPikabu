namespace Code.Gameplay.Features.GameOver.Service
{
    public interface IGameOverService
    {
        void GameWin();
        void GameOver();
        bool IsGameWin { get; }
    }
}