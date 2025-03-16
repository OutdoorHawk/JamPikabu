using System.Collections.Generic;
using System.Threading;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency.Service;
using Code.Meta.Features.Days.Configs.Stars;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.Days.UIService;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;
using static System.Threading.CancellationTokenSource;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.Currency.Behaviours
{
    public class RatingProgressBar : MonoBehaviour
    {
        public SlicedFilledImage BarWithdrawImage;
        public SlicedFilledImage BarFillImage;
        public RectTransform RatingFlyPos;
        public TMP_Text ProgressText;
        public RectTransform Container;
        public float FillBarDuration = 0.6f;
        public float WithdrawDuration = 0.3f;

        private int _currentPointsAmount;
        private int _currentWithdrawAmount;
        private int _maxRatingInDay;

        private RatingBarStarItem[] _items;

        private IDaysService _daysService;
        private IGameplayCurrencyService _gameplayCurrencyService;
        private IDaysUIService _daysUIService;
        
        private CancellationTokenSource _barToken;

        private const float CurrencyFlyDelay = 1.4f;

        [Inject]
        private void Construct
        (
            IDaysService daysService,
            IDaysUIService daysUIService,
            IGameplayCurrencyService gameplayCurrencyService
        )
        {
            _daysUIService = daysUIService;
            _gameplayCurrencyService = gameplayCurrencyService;
            _daysService = daysService;
        }

        private void Awake()
        {
            ResetToken();
        }

        private void OnEnable()
        {
            _daysService.OnDayBegin += Init;
            _gameplayCurrencyService.CurrencyChanged += Refresh;
        }

        private void OnDisable()
        {
            _daysService.OnDayBegin -= Init;
            _gameplayCurrencyService.CurrencyChanged -= Refresh;
        }

        private void Init()
        {
            if (_daysUIService.CheckLevelHasStars() == false)
            {
                gameObject.DisableElement();
                return;
            }

            CreateItems(_daysService.DayStarsData);
            InitText();
            BarWithdrawImage.fillAmount = 0;
            BarFillImage.fillAmount = 0;
        }

        private void CreateItems(List<DayStarData> values)
        {
            _maxRatingInDay = _daysService.GetDayStarData().RatingNeedAll;
            _items = Container.GetComponentsInChildren<RatingBarStarItem>();

            for (int i = 0; i < _items.Length; i++)
            {
                RatingBarStarItem ratingBarStarItem = _items[i];
                ratingBarStarItem.Init(values[i]);
            }
        }

        private void InitText()
        {
            ProgressText.text = $"{0}/{_maxRatingInDay}";
        }

        private void Refresh()
        {
            RefreshDelayed().Forget();
        }

        private async UniTaskVoid RefreshDelayed()
        {
            await DelaySeconds(CurrencyFlyDelay, _barToken.Token);

            int currentRating = _gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Plus, false);
            currentRating -= _gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Minus, false);

            if (_currentPointsAmount == currentRating)
                return;

            ResetToken();
            RefreshFillBar(currentRating, BarFillImage, FillBarDuration);
            RefreshFillBar(currentRating, BarWithdrawImage, WithdrawDuration);
            RefreshText(currentRating);
            _currentPointsAmount = currentRating;
            UpdateItemsAnimation().Forget();
        }

        private void RefreshFillBar(int currentRating, SlicedFilledImage filledImage, float duration)
        {
            float factor = (float)currentRating / _maxRatingInDay;
            factor = Mathf.Clamp(factor, 0, 1);

            /*if (Mathf.Approximately(factor, filledImage.rectTransform.localScale.x))
                return;*/

            filledImage.ToFillAmount
            (
                factor,
                duration,
                _barToken.Token
            ).Forget();
        }

        private async UniTaskVoid UpdateItemsAnimation()
        {
            foreach (RatingBarStarItem item in _items)
            {
                await DelaySeconds(FillBarDuration / _items.Length, _barToken.Token);

                if (_currentPointsAmount >= item.RatingAmount)
                {
                    item.PlayReplenish();
                }
                else
                {
                    item.ResetReplenish();
                }
            }
        }

        private void RefreshText(int currentRating)
        {
            ProgressText.ToDoubleIntArgText
            (
                _currentPointsAmount,
                currentRating,
                _maxRatingInDay,
                "{0}/{1}",
                FillBarDuration,
                _barToken.Token
            ).Forget();
        }

        private void ResetToken()
        {
            _barToken?.Cancel();
            _barToken = CreateLinkedTokenSource(destroyCancellationToken);
        }
    }
}