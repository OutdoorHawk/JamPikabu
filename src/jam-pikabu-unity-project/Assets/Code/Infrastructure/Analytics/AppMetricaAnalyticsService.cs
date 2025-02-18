using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using GamePush;
using Io.AppMetrica;
using Io.AppMetrica.Profile;
using UnityEngine;

namespace Code.Infrastructure.Analytics
{
    public class AppMetricaAnalyticsService : BaseAnalyticsService,
        IEnterBootstrapStateHandler,
        IMainMenuStateHandler
    {
        public OrderType OrderType => OrderType.Last;

        private const string API_KEY = "ea54dbaf-6823-4663-8ebf-a6ca3166983b";
        private const string FIRST_LAUNCH_KEY = "first_launch";

        public void OnEnterBootstrap()
        {
            AppMetrica.Activate(new AppMetricaConfig(API_KEY)
            {
                Logs = true,
                SessionTimeout = 60,
                FirstActivationAsUpdate = !IsFirstLaunch()
            });

            AppMetrica.SetUserProfileID(GP_Player.GetID().ToString());
            GP_Analytics.Hit(Application.absoluteURL);
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
            SendEventInternal(eventName, value);
        }

        public override void SendEventAds(string eventName)
        {
            base.SendEventAds(eventName);
            string eventParameters = "{\"ad_type\":\"" + _adsType + "\"}";
            AppMetrica.ReportEvent(eventName, eventParameters);
            _adsType = AdsEventTypes.Unknown;
        }

        private void SendEventInternal(string eventName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                AppMetrica.ReportEvent(eventName);
            }
            else
            {
                string eventParameters = "{\"value\":\"" + value + "\"}";
                AppMetrica.ReportEvent(eventName, eventParameters);
            }

            if (eventName.Equals(AnalyticsEventTypes.LevelEnd))
                SendUserPassedLevel();
        }

        private void SendUserPassedLevel()
        {
            var userProfile = new UserProfile()
                .Apply(Attribute.CustomCounter("passed_levels")
                    .WithDelta(1));

            AppMetrica.ReportUserProfile(userProfile);
        }

        private static bool IsFirstLaunch()
        {
            if (PlayerPrefs.HasKey(FIRST_LAUNCH_KEY))
                return false;

            PlayerPrefs.SetInt(FIRST_LAUNCH_KEY, 1);
            PlayerPrefs.Save();
            return true;
        }
    }
}