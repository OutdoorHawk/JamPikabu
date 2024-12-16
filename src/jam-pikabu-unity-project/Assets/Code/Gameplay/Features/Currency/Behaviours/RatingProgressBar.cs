using System.Collections.Generic;
using System.Threading;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency.Service;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static System.Threading.CancellationTokenSource;

namespace Code.Gameplay.Features.Currency.Behaviours
{
    public class RatingProgressBar : MonoBehaviour
    {
        public Image BarBackImage;
        public Image BarFillImage;
        public RectTransform AnchorBack;
        public RectTransform RatingFlyPos;
        public TMP_Text ProgressText;
        public RectTransform Container;
        public RatingBarStarItem Prefab;
        public float FillBarDuration = 0.5f;

        private int _currentPointsAmount;
        private int _currentWithdrawAmount;
        private int _maxRatingInDay;

        private readonly List<RatingBarStarItem> _items = new();

        private IDaysService _daysService;
        private IInstantiator _instantiator;
        private IGameplayCurrencyService _gameplayCurrencyService;
        private Tweener _tween;
        private CancellationTokenSource _textAnimToken;

        [Inject]
        private void Construct
        (
            IDaysService daysService,
            IInstantiator instantiator,
            IGameplayCurrencyService gameplayCurrencyService
        )
        {
            _gameplayCurrencyService = gameplayCurrencyService;
            _instantiator = instantiator;
            _daysService = daysService;
        }

        private void Awake()
        {
            Vector3 scale = BarFillImage.rectTransform.localScale;
            BarFillImage.rectTransform.localScale = scale.SetX(0);
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
            List<DayStarData> values = dayData.Stars;

            if (values == null || values.Count == 0)
            {
                gameObject.DisableElement();
                return;
            }

            CreateItems(values);
            InitText();
        }

        private void CreateItems(List<DayStarData> values)
        {
            foreach (var item in _items)
                Destroy(item.gameObject);

            _items.Clear();

            _maxRatingInDay = values[^1].RatingAmountNeed;

            foreach (DayStarData value in values)
            {
                PlaceElement(value);
            }
        }

        private void PlaceElement(in DayStarData data)
        {
            // Создаем элемент
            var element = _instantiator.InstantiatePrefabForComponent<RatingBarStarItem>(Prefab, Container);
            _items.Add(element);
            element.Init(in data);

            // Расчет позиции в прогресс-баре
            float normalizedPosition = (float)data.RatingAmountNeed / _maxRatingInDay; // Значение от 0 до 1
            float xPosition = normalizedPosition * AnchorBack.rect.width;

            // Устанавливаем позицию элемента
            RectTransform elementRect = element.GetComponent<RectTransform>();
            elementRect.anchoredPosition = new Vector2(xPosition, 0);
        }

        private void InitText()
        {
            ProgressText.text = $"{0}/{_maxRatingInDay}";
        }

        private void Refresh()
        {
            if (_maxRatingInDay == 0)
                return;

            int currentRating = _gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Plus);
            currentRating -= _gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Minus);

            if (_currentPointsAmount == currentRating)
                return;

            RefreshFillBar(currentRating);
            RefreshText(currentRating);
            _currentPointsAmount = currentRating;
            UpdateItemsAnimation();
        }

        private void RefreshFillBar(int currentRating)
        {
            float factor = (float)currentRating / _maxRatingInDay;
            factor = Mathf.Clamp(factor, 0, 1);

            if (Mathf.Approximately(factor, BarFillImage.rectTransform.localScale.x))
                return;

            _tween?.Kill();
            _tween = BarFillImage.rectTransform
                    .DOScaleX(factor, FillBarDuration)
                    .SetLink(gameObject)
                ;
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
            _textAnimToken?.Cancel();
            _textAnimToken = CreateLinkedTokenSource(destroyCancellationToken);
            
            ProgressText.ToDoubleIntArgText
            (
                _currentPointsAmount,
                currentRating,
                _maxRatingInDay,
                "{0}/{1}",
                FillBarDuration,
                _textAnimToken.Token
            ).Forget();
        }
    }
}