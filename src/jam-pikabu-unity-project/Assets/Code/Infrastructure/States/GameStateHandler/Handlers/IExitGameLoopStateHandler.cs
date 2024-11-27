namespace Code.Infrastructure.States.GameStateHandler.Handlers
{
    public interface IExitGameLoopStateHandler : IOrderableHandler
    {
        public void OnExitGameLoop();
    }
}