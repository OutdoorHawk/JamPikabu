namespace Code.Infrastructure.States.GameStateHandler.Handlers
{
    public interface IMainMenuStateHandler : IOrderableHandler
    {
        public void OnEnterMainMenu();
        public void OnExitMainMenu();
    }
}