using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using GamePush;
using UnityEngine;

namespace Code.Infrastructure.Analytics
{
    public class GamePushAnalyticsService : BaseAnalyticsService, IMainMenuStateHandler, ILoadProgressStateHandler
    {
        public OrderType StateHandlerOrder => OrderType.Last;

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

        public void OnExitMainMenu()
        {
            
        }

        public override void SendEvent(string eventName, string value)
        {
            base.SendEvent(eventName, value);
            GP_Analytics.Goal(eventName, value);
        }

        public override void SendEventAds(string eventName)
        {
            base.SendEventAds(eventName);
            GP_Analytics.Goal(eventName, _adsType.ToString());
            _adsType = AdsEventTypes.Unknown;
        }
    }
}