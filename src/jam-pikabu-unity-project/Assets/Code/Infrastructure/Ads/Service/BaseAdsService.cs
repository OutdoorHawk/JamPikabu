using System.Collections.Generic;
using Code.Common.Ads.Handler;
using Code.Common.Logger.Service;
using Code.Infrastructure.Common;
using Zenject;

namespace Code.Infrastructure.Ads.Service
{
    public class BaseAdsService : IAdsService
    {
        private readonly BufferedList<IAdsStartedHandler> _startedHandlers = new();
        private readonly BufferedList<IAdsErrorHandler> _errorHandlers = new();
        private readonly BufferedList<IAdsSuccsessfulHandler> _succsessfulHandlers = new();
        private ILoggerService _logger;

        protected ILoggerService Logger => _logger;

        [Inject]
        private void Construct
        (
            List<IAdsStartedHandler> startedHandlers,
            List<IAdsErrorHandler> errorHandlers,
            List<IAdsSuccsessfulHandler> succsessfulHandlers,
            ILoggerService logger
        )
        {
            _logger = logger;
            _succsessfulHandlers.AddRange(succsessfulHandlers);
            _errorHandlers.AddRange(errorHandlers);
            _startedHandlers.AddRange(startedHandlers);
        }

        public void RegisterAdsHandler(IAdsHandler handler)
        {
            switch (handler)
            {
                case IAdsSuccsessfulHandler succsessfulHandler:
                    _succsessfulHandlers.Add(succsessfulHandler);
                    break;
                case IAdsErrorHandler errorHandler:
                    _errorHandlers.Add(errorHandler);
                    break;
                case IAdsStartedHandler startedHandler:
                    _startedHandlers.Add(startedHandler);
                    break;
            }
        }

        public void UnregisterAdsHandler(IAdsHandler handler)
        {
            switch (handler)
            {
                case IAdsSuccsessfulHandler succsessfulHandler:
                    _succsessfulHandlers.Remove(succsessfulHandler);
                    break;
                case IAdsErrorHandler errorHandler:
                    _errorHandlers.Remove(errorHandler);
                    break;
                case IAdsStartedHandler startedHandler:
                    _startedHandlers.Remove(startedHandler);
                    break;
            }
        }

        public virtual void RequestRewardedAd()
        {
            _logger.Log("[AD] Request Rewarded Ad");
        }

        public virtual void RequestMidGameAd()
        {
        }

        protected void NotifyStartedHandlers()
        {
            _logger.Log("<b>[Ads]</b> Ad started");

            foreach (var adsErrorHandler in _startedHandlers)
            {
                adsErrorHandler.OnAdsStarted();
            }
        }

        protected void NotifySuccsessfulHandlers()
        {
            _logger.Log("<b>[Ads]</b> Ad success");

            foreach (var handler in _succsessfulHandlers)
            {
                handler.OnAdsSuccsessful();
            }
        }

        protected void NotifyErrorHandlers(string error)
        {
            _logger.Log($"<b>[Ads]</b> Ad error {error}");

            foreach (var handler in _errorHandlers)
            {
                handler.OnAdsError(error);
            }
        }
    }
}