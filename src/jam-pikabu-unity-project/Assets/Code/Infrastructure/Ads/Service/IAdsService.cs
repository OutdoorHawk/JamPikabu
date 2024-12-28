using Code.Common.Ads.Handler;

namespace Code.Infrastructure.Ads.Service
{
    public interface IAdsService
    {
        bool CanShowRewarded { get; }
        bool CanShowInterstitial { get; }
        bool CanShowBanner { get; }
        bool CanShowPreload { get; }
        void SetupIdentifier(string id);
        void RegisterAdsHandler(IAdsHandler handler);
        void UnregisterAdsHandler(IAdsHandler handler);
        void RequestRewardedAd();
        void RequestInterstitial();
        void RequestBanner();
    }
}