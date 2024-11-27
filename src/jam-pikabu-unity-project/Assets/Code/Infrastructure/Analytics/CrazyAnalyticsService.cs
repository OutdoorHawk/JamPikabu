using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using CrazyGames;

namespace Code.Infrastructure.Analytics
{
    public class CrazyAnalyticsService : IAnalyticsService,
        IEnterGameLoopStateHandler,
        IExitGameLoopStateHandler
    {
        public OrderType OrderType => OrderType.Last;

        public void OnEnterGameLoop()
        {
            CrazySDK.Game.GameplayStart();
        }

        public void OnExitGameLoop()
        {
            CrazySDK.Game.GameplayStop();
        }
    }
}