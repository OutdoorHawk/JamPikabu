using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using CrazyGames;

namespace Code.Infrastructure.Ads.Service
{
    public class CrazyGamesAdsService : 
        BaseAdsService, 
        IEnterBootstrapStateHandler,
        IEnterMainMenuStateHandler
    {
        public OrderType OrderType => OrderType.First;

        private int _enterMainMenuCounter = -1;
        
        private const int INTERSTITIAL_AD_MAIN_MENU_COUNT = 3;
        
        public void OnEnterMainMenu()
        {
            _enterMainMenuCounter++;

            if (_enterMainMenuCounter < INTERSTITIAL_AD_MAIN_MENU_COUNT) 
                return;
            
            _enterMainMenuCounter = 0;
            CrazySDK.Ad.RequestAd(CrazyAdType.Midgame, NotifyStartedHandlers, NotyfyError, NotifySuccsessfulHandlers);
        }

        public void OnEnterBootstrap()
        {
            CrazySDK.Init(OnInited);
        }

        private void OnInited()
        {
            if (CrazySDK.IsAvailable == false) 
                Logger.LogError("<b>[Ads]</b> Crazy SDK is not available after init");
            else
                Logger.Log("<b>[Ads]</b> Crazy SDK inited successfully");
        }

        public override void RequestRewardedAd()
        {
            base.RequestRewardedAd();
            
            CrazySDK.Ad.RequestAd(CrazyAdType.Rewarded, NotifyStartedHandlers, NotyfyError, NotifySuccsessfulHandlers);
        }

        private void NotyfyError(SdkError obj)
        {
            NotifyErrorHandlers(obj.ToString());
        }
    }
}