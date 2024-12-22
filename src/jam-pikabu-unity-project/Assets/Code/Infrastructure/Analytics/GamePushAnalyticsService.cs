using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using GamePush;

namespace Code.Infrastructure.Analytics
{
    public class GamePushAnalyticsService : BaseAnalyticsService, IEnterMainMenuStateHandler
    {
        public OrderType OrderType => OrderType.Last;

        public void OnEnterMainMenu()
        {
            SendEvent(AnalyticsEventTypes.MainMenuEnter, string.Empty);
        }

        public override void SendEvent(string eventName, string value)
        {
            base.SendEvent(eventName, value);
            GP_Analytics.Goal(eventName, value);
        }
    }
}