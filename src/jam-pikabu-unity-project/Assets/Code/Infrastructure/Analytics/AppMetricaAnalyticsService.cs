using Code.Infrastructure.ABTesting;
using Code.Infrastructure.Integrations;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using Cysharp.Threading.Tasks;
using GamePush;
using Io.AppMetrica;
using Io.AppMetrica.Profile;
using UnityEngine;

namespace Code.Infrastructure.Analytics
{
    public class AppMetricaAnalyticsService : BaseAnalyticsService,
        IMainMenuStateHandler,
        IIntegration
    {
        private readonly IABTestService _abTestService;

        private const string API_KEY = "ea54dbaf-6823-4663-8ebf-a6ca3166983b";
        private const string FIRST_LAUNCH_KEY = "first_launch";

        public OrderType StateHandlerOrder => OrderType.Last;
        public OrderType InitOrder => OrderType.Last;

        public AppMetricaAnalyticsService(IABTestService abTestService)
        {
            _abTestService = abTestService;
        }
        
        public UniTask Initialize()
        {
            AppMetrica.Activate(new AppMetricaConfig(API_KEY)
            {
                Logs = true,
                SessionTimeout = 60,
                FirstActivationAsUpdate = !IsFirstLaunch()
            });

            AppMetrica.SetUserProfileID(GP_Player.GetID().ToString());
            GP_Analytics.Hit(Application.absoluteURL);
            
            _logger.Log("[Analytics] AppMetrica initialized");
            return UniTask.CompletedTask;
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
            ExperimentValueTypeId experimentValue = _abTestService.GetExperimentValue(ExperimentTagTypeId.TIMER_REPLACE);
            
            var userProfile = new UserProfile()
                .Apply(Attribute.CustomCounter("passed_levels")
                    .WithDelta(1))
                .Apply(Attribute.CustomNumber(nameof(ExperimentTagTypeId.TIMER_REPLACE))
                    .WithValue((int)experimentValue))
                ;

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