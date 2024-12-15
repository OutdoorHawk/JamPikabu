using System;
using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency.Service;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Features.Currency.Behaviours
{
    public class RatingProgressBar : MonoBehaviour
    {
        public Image BarBackImage;
        public Image BarFillImage;
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
            _gameplayCurrencyService.CurrencyChanged += RefreshBar;
        }

        private void OnDestroy()
        {
            _daysService.OnDayBegin -= Init;
            _gameplayCurrencyService.CurrencyChanged -= RefreshBar;
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
        }

        private void CreateItems(List<DayStarData> values)
        {
            foreach (var item in _items)
                Destroy(item.gameObject);

            _items.Clear();

            _maxRatingInDay = values[^1].RatingAmount;

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
            float normalizedPosition = (float)data.RatingAmount / _maxRatingInDay; // Значение от 0 до 1
            float xPosition = normalizedPosition * BarBackImage.rectTransform.rect.width;

            // Устанавливаем позицию элемента
            RectTransform elementRect = element.GetComponent<RectTransform>();
            elementRect.anchoredPosition = new Vector2(xPosition, 0);
        }

        private void RefreshBar()
        {
            if (_maxRatingInDay == 0)
                return;
            
            int currentRating = _gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Plus);
            currentRating += _gameplayCurrencyService.GetCurrencyOfType(CurrencyTypeId.Minus);

            float factor = (float)currentRating / _maxRatingInDay;
            factor = Mathf.Clamp(factor, 0, _maxRatingInDay);

            if (Mathf.Approximately(factor, BarFillImage.rectTransform.localScale.x))
                return;

            _tween?.Kill();
            _tween = BarFillImage.rectTransform
                .DOScaleX(factor, FillBarDuration)
                .SetLink(gameObject)
                ;
        }
    }
}