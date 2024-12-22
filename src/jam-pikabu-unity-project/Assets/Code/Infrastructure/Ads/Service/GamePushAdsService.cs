using System.Collections.Generic;
using Code.Gameplay.Sound.Service;
using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using Code.Meta.Features.Days;
using Code.Meta.Features.Days.Service;
using GamePush;

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

        public GamePushAdsService(IDaysService daysService, ISoundService soundService)
        {
            _soundService = soundService;
            _daysService = daysService;
        }

        #region IStateHandler

        public void OnEnterMainMenu()
        {
            List<DayProgressData> dayProgressData = _daysService.GetDaysProgress();
            
            if (dayProgressData.Count < 1)
                return;
            
            if (CanShowBanner) 
                RequestBanner();
        }

        public void OnExitGameLoop()
        {
            if (CanShowInterstitial) 
                RequestInterstitial();
        }

        #endregion

        #region BaseAds

        public override void RequestInterstitial()
        {
            base.RequestInterstitial();
            
            GP_Ads.ShowFullscreen(onFullscreenStart: Started, onFullscreenClose: Finished);
        }

        public override void RequestRewardedAd()
        {
            base.RequestRewardedAd();
            GP_Ads.ShowRewarded(_identifier, RewardedSuccess, Started, Finished);
        }

        public override void RequestBanner()
        {
            base.RequestBanner();
            
            if (GP_Ads.IsStickyPlaying())
                return;
            
            GP_Ads.ShowSticky();
        }

        #endregion
        
        private void Started()
        {
            _soundService.MuteVolume();
            NotifyStartedHandlers();
        }

        private void RewardedSuccess(string id)
        {
            NotifySuccessfulHandlers();
        }

        private void Finished(bool success)
        {
            if (success == false) 
                NotifyErrorHandlers("");
            
            _soundService.ResetVolume();
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