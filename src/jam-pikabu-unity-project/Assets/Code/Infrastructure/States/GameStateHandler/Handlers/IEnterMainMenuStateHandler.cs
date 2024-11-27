namespace Code.Infrastructure.States.GameStateHandler.Handlers
{
    public interface IEnterMainMenuStateHandler : IOrderableHandler
    {
        public void OnEnterMainMenu();
    }
}