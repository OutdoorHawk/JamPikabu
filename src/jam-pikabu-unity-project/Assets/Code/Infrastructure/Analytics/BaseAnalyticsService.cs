using Code.Common.Logger.Service;
using Zenject;

namespace Code.Infrastructure.Analytics
{
    public abstract class BaseAnalyticsService : IAnalyticsService
    {
        protected ILoggerService _logger;
        protected AdsEventTypes _adsType;

        [Inject]
        private void Construct(ILoggerService logger)
        {
            _logger = logger;
        }

        public void SetAdsType(AdsEventTypes adsType)
        {
            _adsType = adsType;
        }

        public virtual void SendEventAds(string eventName)
        {
            _logger.Log($"[Analytics] {eventName} {_adsType}");
        }
        
        public virtual void SendEvent(string eventName)
        {
            _logger.Log($"[Analytics] {eventName}");
        }

        public virtual void SendEvent(string eventName, string value)
        {
            _logger.Log($"[Analytics] {eventName}: {value}");
        }
    }
}