using Code.Common.Extensions.Animations;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.Currency.Factory;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Result.Behaviours;
using Code.Gameplay.Features.Result.Service;
using Code.Gameplay.Sound;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Service;
using Code.Infrastructure.Ads.Behaviours;
using Code.Infrastructure.Analytics;
using Code.Meta.UI.Common;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.Result.Window
{
    public class ResultWindow : BaseWindow
    {
        public PriceInfo EarnedRatingUp;
        public PriceInfo EarnedRatingDown;
        public Animator[] Stars;
        public TMP_Text EarnedGold;
        public AdsButton WatchAdsButton;
        public Image ProgressBarFill;

        public float RatingInitDuration = 0.5f;
        public float StarsInitDuration = 0.75f;

        public RectTransform LootItemsHolder;
        public ResultLootItem ResultLootItemPrefab;

        private ICurrencyFactory _currencyFactory;
        private IWindowService _windowService;
        private IAnalyticsService _analyticsService;
        private IResultWindowService _resultWindowService;
        private IInstantiator _instantiator;

        [Inject]
        private void Construct
        (
            ICurrencyFactory currencyFactory,
            IWindowService windowService,
            IAnalyticsService analyticsService,
            IResultWindowService resultWindowService,
            IInstantiator instantiator
        )
        {
            _instantiator = instantiator;
            _resultWindowService = resultWindowService;
            _analyticsService = analyticsService;
            _windowService = windowService;
            _currencyFactory = currencyFactory;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Init().Forget();
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

        private async UniTaskVoid Init()
        {
            InitLoot();
            InitRating();
            await DelaySeconds(RatingInitDuration, destroyCancellationToken);
            await InitStars();
            InitEarningsGold();
        }

        private void InitLoot()
        {
            foreach ((LootTypeId type, int amount) in _resultWindowService.GetCollectedLoot())
            {
                var instance = _instantiator.InstantiatePrefabForComponent<ResultLootItem>(ResultLootItemPrefab, LootItemsHolder);
                instance.Setup(type, amount);
            }
        }

        private void InitRating()
        {
            SetupPriceInfo(EarnedRatingUp, CurrencyTypeId.Plus);
            SetupPriceInfo(EarnedRatingDown, CurrencyTypeId.Minus);
        }

        private void InitEarningsGold()
        {
            int amount = _resultWindowService.GetCollectedCurrency(CurrencyTypeId.Gold);
            EarnedGold.text = amount.ToString();
        }

        private async UniTask InitStars()
        {
            int stars = _resultWindowService.GetCollectedCurrency(CurrencyTypeId.Star);
            for (int i = 0; i < stars; i++)
            {
                Stars[i].SetTrigger(AnimationParameter.Replenish.AsHash());
                await DelaySeconds(StarsInitDuration / stars, destroyCancellationToken);
            }
        }

        private void GiveReward()
        {
            WatchAdsButton.Button.enabled = false;
            _analyticsService.SendEvent(AnalyticsEventTypes.DoubleProfitReward);

            int rewardAmount = 0; //TODO: ACTUAL REWARD
            _currencyFactory.CreateAddCurrencyRequest(CurrencyTypeId.Gold, rewardAmount, rewardAmount);

            PlayAnimation(rewardAmount);
            CloseAfterDelay().Forget();
        }

        private void SetupPriceInfo(PriceInfo priceInfo, CurrencyTypeId type)
        {
            int amount = _resultWindowService.GetCollectedCurrency(type);
            priceInfo.SetupPrice(amount, type, true);
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

        private async UniTaskVoid CloseAfterDelay()
        {
            BlockClosing = true;
            await DelaySeconds(2, destroyCancellationToken);
            BlockClosing = false;
            Close();
        }
    }
}