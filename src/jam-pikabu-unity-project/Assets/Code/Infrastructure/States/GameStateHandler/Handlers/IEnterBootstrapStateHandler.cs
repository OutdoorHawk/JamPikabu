namespace Code.Infrastructure.States.GameStateHandler.Handlers
{
    public interface IEnterBootstrapStateHandler : IOrderableHandler
    {
        public void OnEnterBootstrap();
    }
}