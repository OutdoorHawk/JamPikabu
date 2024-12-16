using GamePush;

namespace Code.Infrastructure.Ads.Service
{
    public class GamePushAdsService : BaseAdsService
    {
        public override bool CanShowRewarded => GP_Ads.IsRewardedAvailable();
        public override bool CanShowInterstitial => GP_Ads.IsFullscreenAvailable();
        public override bool CanShowBanner => GP_Ads.IsStickyAvailable();

        public override void RequestRewardedAd()
        {
            base.RequestRewardedAd();
            
            if (GP_Ads.IsRewardPlaying())
                return;
            
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
    }
}