using GamePush;

namespace Code.Infrastructure.Ads.Service
{
    public class GamePushAdsService : BaseAdsService
    {
        public override void RequestRewardedAd()
        {
            base.RequestRewardedAd();
            
            GP_Ads.ShowRewarded(_identifier, Rewarded, Started, Finished);
        }

        private void Rewarded(string id)
        {
            NotifySuccessfulHandlers();
        }

        private void Started()
        {
            NotifyStartedHandlers();
        }

        private void Finished(bool success)
        {
            if (success == false) 
                NotifyErrorHandlers("");
        }
    }
}