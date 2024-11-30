namespace Code.Infrastructure.States.GameStateHandler.Handlers
{
    public interface ILoadLevelStateHandler : IOrderableHandler
    {
        public void OnEnterLoadLevel();
        public void OnExitLoadLevel();
    }
}