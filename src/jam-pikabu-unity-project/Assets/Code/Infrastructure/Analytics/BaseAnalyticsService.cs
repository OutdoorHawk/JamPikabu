using Code.Common.Logger.Service;
using Zenject;

namespace Code.Infrastructure.Analytics
{
    public abstract class BaseAnalyticsService : IAnalyticsService
    {
        private ILoggerService _logger;

        [Inject]
        private void Construct(ILoggerService logger)
        {
            _logger = logger;
        }
        
        public virtual void SendEvent(string eventName)
        {
            _logger.Log($"[Analytics event] {eventName}");
        }

        public virtual void SendEvent(string eventName, string value)
        {
            _logger.Log($"[Analytics event] {eventName}: {value}");
        }
    }
}