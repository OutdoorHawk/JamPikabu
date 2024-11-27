namespace Code.Infrastructure.States.GameStateHandler.Handlers
{
    public interface IEnterGameLoopStateHandler : IOrderableHandler
    {
        public void OnEnterGameLoop();
    }
}