namespace Code.Infrastructure.States.GameStateHandler.Handlers
{
    public interface ILoadProgressStateHandler : IOrderableHandler
    {
        public void OnEnterLoadProgress();
        public void OnExitLoadProgress();
    }
}