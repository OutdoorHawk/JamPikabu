namespace Code.Infrastructure.States.GameStateHandler
{
    public interface IGameStateHandlerService
    {
        void OnEnterLoadProgressState();
        void OnExitLoadProgressState();
        void OnEnterMainMenu();
        void OnExitMainMenu();
        void OnEnterLoadLevel();
        void OnExitLoadLevel();
        void OnEnterGameLoop();
        void OnExitGameLoop();
    }
}