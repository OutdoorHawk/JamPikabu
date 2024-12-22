using System.Collections.Generic;
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

        public GamePushAdsService(IDaysService daysService)
        {
            _daysService = daysService;
        }

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

        private void Started()
        {
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