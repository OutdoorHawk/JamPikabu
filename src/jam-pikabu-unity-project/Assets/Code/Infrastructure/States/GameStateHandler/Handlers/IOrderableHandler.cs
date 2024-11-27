namespace Code.Infrastructure.States.GameStateHandler.Handlers
{
    public interface IOrderableHandler
    {
        public OrderType OrderType { get; }
    }
}