using Code.Infrastructure.Integrations;
using Code.Infrastructure.States.GameStateHandler;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Ads.Service
{
    public class FakeAdsService : BaseAdsService, IIntegration
    {
        public OrderType InitOrder => OrderType.First;

        public UniTask Initialize()
        {
            Logger.Log("<b>[Ads]</b> Fake ads inited successfully");
            return UniTask.CompletedTask;
        }

        public override void RequestRewardedAd()
        {
            base.RequestRewardedAd();
            NotifySuccessfulHandlers();
        }
    }
}