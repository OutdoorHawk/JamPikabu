namespace Code.Infrastructure.Analytics
{
    public interface IAnalyticsService
    {
        void SetAdsType(AdsEventTypes adsType);
        void SendEventAds(string eventName);
        void SendEvent(string eventName);
        void SendEvent(string eventName, string value);
    }
}