using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Sound;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.Ads.Behaviours;
using Code.Infrastructure.Analytics;
using Code.Meta.UI.Common;
using Cysharp.Threading.Tasks;
using Zenject;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.ProfitAds.Window
{
    public class ProfitAdsWindow : BaseWindow
    {
        public AdsButton WatchAdsButton;
        public PriceInfo Earned;

        private ICurrencyFactory _currencyFactory;
        private IWindowService _windowService;
        private IGameplayCurrencyService _gameplayCurrencyService;
        private IAnalyticsService _analyticsService;

        private int _goldAmount;

        [Inject]
        private void Construct
        (
            ICurrencyFactory currencyFactory,
            IWindowService windowService,
            IGameplayCurrencyService gameplayCurrencyService,
            IAnalyticsService analyticsService
        )
        {
            _analyticsService = analyticsService;
            _gameplayCurrencyService = gameplayCurrencyService;
            _windowService = windowService;
            _currencyFactory = currencyFactory;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _goldAmount = _gameplayCurrencyService.CollectedGoldInLevel;
            Earned.SetupPrice(_goldAmount, CurrencyTypeId.Gold, true);
        }

        protected override void SubscribeUpdates()
        {
            base.SubscribeUpdates();
            WatchAdsButton.OnRewarded += GiveReward;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            WatchAdsButton.OnRewarded -= GiveReward;
        }

        private void GiveReward()
        {
            _analyticsService.SendEvent(AnalyticsEventTypes.DoubleProfitReward);
            WatchAdsButton.Button.enabled = false;

            int rewardAmount = _goldAmount;
            _currencyFactory.CreateAddCurrencyRequest(CurrencyTypeId.Gold, rewardAmount, rewardAmount);
            PlayAnimation(rewardAmount);

            ProceedAfterDelay().Forget();
        }

        private void PlayAnimation(int amount)
        {
            if (_windowService.TryGetWindow(out PlayerHUDWindow hudWindow) == false)
                return;

            var parameters = new CurrencyAnimationParameters
            {
                Count = amount,
                Type = CurrencyTypeId.Gold,
                StartPosition = WatchAdsButton.transform.position,
                EndPosition = hudWindow.CurrencyHolder.PlayerCurrentGold.CurrencyIcon.transform.position,
                StartReplenishCallback = () => _currencyFactory.CreateAddCurrencyRequest(CurrencyTypeId.Gold, 0, -amount),
                BeginAnimationSound = SoundTypeId.Gold_Currency_Collect
            };

            _currencyFactory.PlayCurrencyAnimation(parameters);
        }

        private async UniTaskVoid ProceedAfterDelay()
        {
            BlockClosing = true;
            await DelaySeconds(2, destroyCancellationToken);
            BlockClosing = false;
            Close();
        }
    }
}