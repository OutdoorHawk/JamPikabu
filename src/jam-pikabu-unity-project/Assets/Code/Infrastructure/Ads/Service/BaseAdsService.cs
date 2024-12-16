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
        private readonly BufferedList<IAdsSuccsessfulHandler> _successfulHandlers = new();
        
        private ILoggerService _logger;

        protected ILoggerService Logger => _logger;

        protected string _identifier;
        
        public virtual bool CanShowRewarded => true;
        public virtual bool CanShowInterstitial => true;
        public virtual bool CanShowBanner => true;

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
            _successfulHandlers.AddRange(succsessfulHandlers);
            _errorHandlers.AddRange(errorHandlers);
            _startedHandlers.AddRange(startedHandlers);
        }

        #region Handlers

        public void SetupIdentifier(string id)
        {
            _identifier = id;
        }

        public void RegisterAdsHandler(IAdsHandler handler)
        {
            switch (handler)
            {
                case IAdsSuccsessfulHandler succsessfulHandler:
                    _successfulHandlers.Add(succsessfulHandler);
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
                    _successfulHandlers.Remove(succsessfulHandler);
                    break;
                case IAdsErrorHandler errorHandler:
                    _errorHandlers.Remove(errorHandler);
                    break;
                case IAdsStartedHandler startedHandler:
                    _startedHandlers.Remove(startedHandler);
                    break;
            }
        }

        #endregion

        #region IAdsService

        public virtual void RequestRewardedAd()
        {
            _logger.Log("[AD] Request Rewarded Ad");
        }

        public virtual void RequestInterstitial()
        {
        }

        public virtual void RequestBanner()
        {
            _logger.Log("[AD] Request Banner");
        }

        #endregion

        #region Internal

        protected void NotifyStartedHandlers()
        {
            _logger.Log("<b>[Ads]</b> Ad started");

            foreach (var adsErrorHandler in _startedHandlers)
            {
                adsErrorHandler.OnAdsStarted();
            }
        }

        protected void NotifySuccessfulHandlers()
        {
            _logger.Log("<b>[Ads]</b> Ad success");

            foreach (var handler in _successfulHandlers)
            {
                handler.OnAdsSuccsessful();
            }

            ClearIdentifier();
        }

        protected void NotifyErrorHandlers(string error)
        {
            _logger.Log($"<b>[Ads]</b> Ad error {error}");

            foreach (var handler in _errorHandlers)
            {
                handler.OnAdsError(error);
            }
            
            ClearIdentifier();
        }

        private void ClearIdentifier()
        {
            _identifier = null;
        }

        #endregion
    }
}