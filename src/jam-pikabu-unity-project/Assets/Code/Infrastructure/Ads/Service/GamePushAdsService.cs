using System.Collections.Generic;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.StaticData;
using Code.Gameplay.Tutorial.Service;
using Code.Infrastructure.Ads.Config;
using Code.Infrastructure.Analytics;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using Code.Meta.Features.Days;
using Code.Meta.Features.Days.Service;
using GamePush;
using UnityEngine;
using static Code.Infrastructure.Analytics.AnalyticsEventTypes;

namespace Code.Infrastructure.Ads.Service
{
    public class GamePushAdsService : BaseAdsService, IExitGameLoopStateHandler, IEnterMainMenuStateHandler
    {
#if !UNITY_EDITOR
        public override bool CanShowRewarded => IsRewardedAvailable();
#endif
        public override bool CanShowInterstitial => IsFullscreenAvailable();

        public override bool CanShowBanner => IsStickyAvailable();

        public OrderType OrderType => OrderType.Last;

        private readonly IDaysService _daysService;
        private readonly ISoundService _soundService;
        private readonly ITutorialService _tutorialService;
        private readonly IStaticDataService _staticDataService;
        private readonly IAnalyticsService _analyticsService;

        private bool _firstEnter = true;

        public GamePushAdsService
        (
            IDaysService daysService,
            ISoundService soundService,
            ITutorialService tutorialService,
            IStaticDataService staticDataService,
            IAnalyticsService analyticsService
        )
        {
            _staticDataService = staticDataService;
            _analyticsService = analyticsService;
            _soundService = soundService;
            _tutorialService = tutorialService;
            _daysService = daysService;
        }

        #region IStateHandler

        public void OnEnterMainMenu()
        {
            if (_firstEnter)
            {
                _firstEnter = false;
                return;
            }
            
            List<DayProgressData> dayProgressData = _daysService.GetDaysProgress();
            AdsStaticData adsStaticData = _staticDataService.GetStaticData<AdsStaticData>();

            if (dayProgressData.Count < adsStaticData.LevelsPassedToStartAds)
                return;

            if (_tutorialService.HasActiveTutorial())
                return;

            if (CanShowInterstitial)
            {
                RequestInterstitial();
                return;
            }

            if (CanShowBanner)
                RequestBanner();
        }

        public void OnExitGameLoop()
        {
        }

        #endregion

        #region BaseAds

        public override void RequestInterstitial()
        {
            base.RequestInterstitial();
            
            _analyticsService.SetAdsType(adsType: AdsEventTypes.Interstitial);
            GP_Ads.ShowFullscreen(onFullscreenStart: Started, onFullscreenClose: Finished);
        }

        public override void RequestRewardedAd()
        {
            base.RequestRewardedAd();
            
            _analyticsService.SetAdsType(adsType: AdsEventTypes.Rewarded);
            GP_Ads.ShowRewarded(_identifier, RewardedSuccess, Started, Finished);
        }

        public override void RequestBanner()
        {
            base.RequestBanner();

            if (GP_Ads.IsStickyPlaying())
                return;

            _analyticsService.SetAdsType(adsType: AdsEventTypes.Banner);
            _analyticsService.SendEventAds(AdStarted);
            GP_Ads.ShowSticky();
        }

        #endregion

        private void Started()
        {
            _analyticsService.SendEventAds(AdStarted);
            _soundService.MuteVolume();
            Time.timeScale = 0;
            NotifyStartedHandlers();
        }

        private void RewardedSuccess(string id)
        {
            _analyticsService.SendEvent(AdRewardedSuccess);
            NotifySuccessfulHandlers();
            Time.timeScale = 1;
        }

        private void Finished(bool success)
        {
            if (success == false)
                NotifyErrorHandlers("");

            _soundService.ResetVolume();
            Time.timeScale = 1;
        }

        private bool IsFullscreenAvailable()
        {
            if (GP_Ads.IsFullscreenPlaying())
                return false;

            bool result = GP_Ads.IsFullscreenAvailable();
            Logger.Log($"[AD] IsFullscreenAvailable: {result}");
            return result;
        }

        private bool IsStickyAvailable()
        {
            bool result = GP_Ads.IsStickyAvailable();
            Logger.Log($"[AD] IsStickyAvailable: {result}");
            return result;
        }

        private bool IsRewardedAvailable()
        {
            if (GP_Ads.IsRewardPlaying())
                return false;

            bool result = GP_Ads.IsRewardedAvailable();
            Logger.Log($"[AD] IsRewardedAvailable: {result}");
            return result;
        }
    }
}