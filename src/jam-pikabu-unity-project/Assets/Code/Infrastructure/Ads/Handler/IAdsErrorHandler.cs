namespace Code.Common.Ads.Handler
{
    public interface IAdsErrorHandler : IAdsHandler
    {
        void OnAdsError(string error);
    }
}