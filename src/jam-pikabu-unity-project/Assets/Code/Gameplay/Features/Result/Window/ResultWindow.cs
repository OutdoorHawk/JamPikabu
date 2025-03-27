using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Behaviours;
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
using Code.Infrastructure.Ads.Service;
using Code.Infrastructure.Analytics;
using Code.Meta.Features.BonusLevel.Config;
using Code.Meta.Features.Days.Configs.Stars;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.Days.UIService;
using Code.Meta.UI.Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.Result.Window
{
    public class ResultWindow : BaseWindow
    {
        public PriceInfo EarnedRatingUp;
        public PriceInfo EarnedRatingDown;
        public TMP_Text EarnedGold;
        public TMP_Text TitleText;
        public AdsButton WatchAdsButton;
        public SlicedFilledImage ProgressBarFill;
        public GameObject ProgressBar;
        public CanvasGroup EarningsStarsContainer;
        public RatingBarStarItem[] StarsProgressBar;
        public Animator[] ResultStars;

        public float AwakeInitDuration = 0.5f;
        public float AdsButtonShowDelay = 0.5f;
        public float FillBarDuration = 1f;
        public int LootAmountInRow = 4;

        public RectTransform[] LootRow;
        public ResultLootItem ResultLootItemPrefab;

        private ICurrencyFactory _currencyFactory;
        private IWindowService _windowService;
        private IAnalyticsService _analyticsService;
        private IResultWindowService _resultWindowService;
        private IInstantiator _instantiator;
        private IDaysService _daysService;
        private IDaysUIService _daysUIService;
        private IAdsService _adsService;

        private int _currentDay;

        private const string WIN_KEY = "GO_RESULT_WIN";
        private const string LOST_KEY = "GO_RESULT_LOSE";
        
        private bool IsBonusLevel => _daysService.BonusLevelType != BonusLevelType.None;

        [Inject]
        private void Construct
        (
            ICurrencyFactory currencyFactory,
            IWindowService windowService,
            IAnalyticsService analyticsService,
            IResultWindowService resultWindowService,
            IInstantiator instantiator,
            IDaysService daysService,
            IDaysUIService daysUIService,
            IAdsService adsService
        )
        {
            _daysUIService = daysUIService;
            _adsService = adsService;
            _daysService = daysService;
            _instantiator = instantiator;
            _resultWindowService = resultWindowService;
            _analyticsService = analyticsService;
            _windowService = windowService;
            _currencyFactory = currencyFactory;
        }

        protected override void Initialize()
        {
            base.Initialize();
           
            _currentDay = _resultWindowService.CurrentDay;
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
            InitTitle();
            InitRating();
            InitProgressBar();
            InitProgressBarStars();
            InitEarningsGold();
            InitAdsButton();
            InitEarningsStars();
            await DelaySeconds(AwakeInitDuration, destroyCancellationToken);
            PlayRating();
            PlayProgressBarFill();
            PlayStarsAnimation().Forget();
            PlayLoot();
            PlayEarningsStars();
            PlayEarningsGold();
            await DelaySeconds(AdsButtonShowDelay, destroyCancellationToken);
            PlayAdsButton();
        }

        private void InitTitle()
        {
            if (IsBonusLevel)
            {
                TitleText.text = LocalizationService[$"GAME OVER/{WIN_KEY}"];
                return;
            }
            
            TitleText.text = _resultWindowService.CheckGameWin() 
                ? LocalizationService[$"GAME OVER/{WIN_KEY}"] 
                : LocalizationService[$"GAME OVER/{LOST_KEY}"];
        }

        private void InitRating()
        {
            if (IsBonusLevel)
            {
                EarnedRatingUp.DisableElement();
                EarnedRatingDown.DisableElement();
                return;
            }
            
            EarnedRatingUp.SetupPrice(0, CurrencyTypeId.Plus);
            EarnedRatingDown.SetupPrice(0, CurrencyTypeId.Minus);
        }

        private void InitProgressBar()
        {
            ProgressBarFill.fillAmount = 0;
        }

        private void InitProgressBarStars()
        {
            if (_daysUIService.CheckLevelHasStars() == false)
                return;

            List<DayStarData> values = _daysService.DayStarsData;

            for (int i = 0; i < StarsProgressBar.Length; i++)
            {
                RatingBarStarItem ratingBarStarItem = StarsProgressBar[i];
                ratingBarStarItem.Init(values[i]);
            }
        }

        private void InitEarningsGold()
        {
            EarnedGold
                .DOFade(0, 0)
                .SetLink(gameObject);
        }

        private void InitAdsButton()
        {
            if (_adsService.CanShowRewarded) 
                WatchAdsButton.transform.localScale = Vector3.zero;
            else
                WatchAdsButton.DisableElement();
        }

        private void InitEarningsStars()
        {
            if (IsBonusLevel)
            {
                EarningsStarsContainer.DisableElement();
                return;
            }
            
            EarningsStarsContainer.alpha = 0;
            
            foreach (Animator star in ResultStars) 
                star.DisableElement();
        }

        private void PlayRating()
        {
            SetupPriceInfo(EarnedRatingUp, CurrencyTypeId.Plus);
            SetupPriceInfo(EarnedRatingDown, CurrencyTypeId.Minus);
        }

        private void PlayProgressBarFill()
        {
            int rating = _resultWindowService.GetTotalRating();
            int maxRatingInDay = _daysService.GetDayStarData(_currentDay).RatingNeedAll;

            float factor = (float)rating / maxRatingInDay;
            factor = Mathf.Clamp(factor, 0, 1);

            ProgressBarFill.ToFillAmount
            (
                factor,
                FillBarDuration,
                destroyCancellationToken
            ).Forget();
        }

        private async UniTaskVoid PlayStarsAnimation()
        {
            int rating = _resultWindowService.GetTotalRating();

            for (int i = 0; i < StarsProgressBar.Length; i++)
            {
                RatingBarStarItem item = StarsProgressBar[i];
                await DelaySeconds(FillBarDuration / StarsProgressBar.Length, destroyCancellationToken);

                if (rating >= item.RatingAmount)
                {
                    item.PlayReplenish();
                    ResultStars[i].EnableElement();
                }
                else
                {
                    ResultStars[i].DisableElement();
                    item.ResetReplenish();
                }
            }
        }

        private void PlayLoot()
        {
            int rowIndex = 0;
            int lootCount = 0;
            foreach ((LootTypeId type, int amount) in _resultWindowService.GetCollectedLoot())
            {
                if (type is LootTypeId.WoodChip)
                    continue;
                
                if (lootCount >= LootAmountInRow && rowIndex < LootRow.Length)
                {
                    rowIndex++;
                    lootCount = 0;
                }

                var instance = _instantiator.InstantiatePrefabForComponent<ResultLootItem>(ResultLootItemPrefab, LootRow[rowIndex]);
                instance.Setup(type, amount);
                lootCount++;
            }
        }

        private void PlayEarningsStars()
        {
            if (_daysUIService.CheckLevelHasStars() == false)
                return;
            
            EarningsStarsContainer.DOFade(1, 0.5f)
                .SetLink(gameObject);
        }

        private void PlayEarningsGold()
        {
            EarnedGold
                .DOFade(1, 0.25f)
                .SetLink(gameObject);

            int amount = _resultWindowService.GetCollectedCurrency(CurrencyTypeId.Gold);
            EarnedGold.text = amount.ToString();
        }

        private void PlayAdsButton()
        {
            WatchAdsButton.transform
                .DOScale(Vector3.one, 0.4f)
                .SetEase(Ease.OutBounce)
                .SetLink(gameObject);
        }

        private void GiveReward()
        {
            WatchAdsButton.Button.enabled = false;
            _analyticsService.SendEvent(AnalyticsEventTypes.DoubleProfitReward);

            int rewardAmount = _resultWindowService.GetCollectedCurrency(CurrencyTypeId.Gold);
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
                StartPosition = WatchAdsButton.transform.position + new Vector3(0, 5, 0),
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