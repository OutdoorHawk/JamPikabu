using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using GamePush;
using UnityEngine;

namespace Code.Infrastructure.Analytics
{
    public class GamePushAnalyticsService : BaseAnalyticsService, IEnterMainMenuStateHandler, ILoadProgressStateHandler
    {
        public OrderType OrderType => OrderType.Last;

        public void OnEnterLoadProgress()
        {
            GP_Analytics.Hit(Application.absoluteURL);
        }

        public void OnExitLoadProgress()
        {
        }

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