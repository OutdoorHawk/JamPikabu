using Code.Common.Ads.Handler;

namespace Code.Infrastructure.Ads.Service
{
    public interface IAdsService
    {
        void SetupIdentifier(string id);
        void RegisterAdsHandler(IAdsHandler handler);
        void UnregisterAdsHandler(IAdsHandler handler);
        void RequestRewardedAd();
        void RequestMidGameAd();
    }
}