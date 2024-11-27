using Code.Infrastructure.States.GameStateHandler;
using Code.Infrastructure.States.GameStateHandler.Handlers;
using CrazyGames;

namespace Code.Infrastructure.Ads.Service
{
    public class FakeAdsService : BaseAdsService, IEnterBootstrapStateHandler
    {
        public OrderType OrderType => OrderType.First;

        public void OnEnterBootstrap()
        {
            Logger.Log("<b>[Ads]</b> Fake ads inited successfully");
        }

        public override void RequestRewardedAd()
        {
            base.RequestRewardedAd();
            NotifySuccsessfulHandlers();
        }
    }
}