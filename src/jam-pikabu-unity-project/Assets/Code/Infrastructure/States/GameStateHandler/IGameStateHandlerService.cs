using Code.Infrastructure.States.GameStateHandler.Handlers;

namespace Code.Infrastructure.States.GameStateHandler
{
    public interface IGameStateHandlerService
    {
        void RegisterHandler(IOrderableHandler handler);
        void OnEnterBootstrapState();
        void OnEnterLoadProgressState();
        void OnExitLoadProgressState();
        void OnEnterMainMenu();
        void OnEnterLoadLevel();
        void OnExitLoadLevel();
        void OnEnterGameLoop();
        void OnExitGameLoop();
    }
}