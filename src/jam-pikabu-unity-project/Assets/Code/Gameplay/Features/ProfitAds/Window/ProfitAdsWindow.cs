using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.Ads.Behaviours;
using Cysharp.Threading.Tasks;
using Entitas;
using Zenject;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.ProfitAds.Window
{
    public class ProfitAdsWindow : BaseWindow
    {
        public AdsButton WatchAdsButton;

        private GameContext _gameContext;
        private ICurrencyFactory _currencyFactory;
        private IWindowService _windowService;

        [Inject]
        private void Construct
        (
            GameContext gameContext,
            ICurrencyFactory currencyFactory,
            IWindowService windowService
        )
        {
            _windowService = windowService;
            _currencyFactory = currencyFactory;
            _gameContext = gameContext;
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
            IGroup<GameEntity> group = _gameContext.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Gold,
                    GameMatcher.CurrencyStorage,
                    GameMatcher.GoldPerDay));

            foreach (var storage in group)
            {
                storage.ReplaceGold(storage.Gold + storage.GoldPerDay);
                PlayAnimation(storage.GoldPerDay);
            }

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