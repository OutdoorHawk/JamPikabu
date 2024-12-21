using System.Collections.Generic;
using System.Threading;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency.Service;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Configs.Stars;
using Code.Meta.Features.Days.Service;
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
        private CancellationTokenSource _barToken;

        [Inject]
        private void Construct
        (
            IDaysService daysService,
            IGameplayCurrencyService gameplayCurrencyService
        )
        {
            _gameplayCurrencyService = gameplayCurrencyService;
            _daysService = daysService;
        }

        private void Start()
        {
            _daysService.OnDayBegin += Init;
            _gameplayCurrencyService.CurrencyChanged += Refresh;
        }

        private void OnDestroy()
        {
            _daysService.OnDayBegin -= Init;
            _gameplayCurrencyService.CurrencyChanged -= Refresh;
        }

        private void Init()
        {
            DayData dayData = _daysService.GetDayData();
            DayStarsSetup dayStarsSetup = _daysService.GetDayStarData(dayData.Id);
            List<DayStarData> values = dayStarsSetup.Stars;

            if (values == null || values.Count == 0)
            {
                gameObject.DisableElement();
                return;
            }

            CreateItems(values);
            InitText();
            BarWithdrawImage.fillAmount = 0;
            BarFillImage.fillAmount = 0;
        }

        private void CreateItems(List<DayStarData> values)
        {
            _maxRatingInDay = values[^1].RatingAmountNeed;
            _items = Container.GetComponentsInChildren<RatingBarStarItem>();
            DayStarsSetup dayStarsSetup = _daysService.GetDayStarData();
            for (int i = 0; i < _items.Length; i++)
            {
                RatingBarStarItem ratingBarStarItem = _items[i];
                ratingBarStarItem.Init(dayStarsSetup.Stars[i]);
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
            if (_maxRatingInDay == 0)
                return;

            ResetToken();

            const float currencyFlyDelay = 1.5f;
            await DelaySeconds(currencyFlyDelay, _barToken.Token);

            int currentRating = _gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Plus, false);
            currentRating -= _gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Minus, false);

            if (_currentPointsAmount == currentRating)
                return;

            RefreshFillBar(currentRating, BarFillImage, FillBarDuration);
            RefreshFillBar(currentRating, BarWithdrawImage, WithdrawDuration);
            RefreshText(currentRating);
            _currentPointsAmount = currentRating;
            UpdateItemsAnimation();
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

        private void UpdateItemsAnimation()
        {
            foreach (RatingBarStarItem item in _items)
            {
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