using Code.Common.Extensions;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.StaticData;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation
{
    public class CurrencyAnimation : MonoBehaviour
    {
        [SerializeField] private Image[] _icons;
        [SerializeField] private RectTransform[] _rects;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _flyDuration = 1;
        [SerializeField] private float _scatterDuration = 1f;

        private IStaticDataService _staticData;
        private bool _firstObject = true;

        [Inject]
        private void Construct(IStaticDataService staticDataService)
        {
            _staticData = staticDataService;
        }

        public void Initialize(in CurrencyAnimationParameters parameters)
        {
            InitIcons(parameters);
            PlayAnimation(parameters);
        }

        private void InitIcons(in CurrencyAnimationParameters parameters)
        {
            CurrencyConfig currencyConfig = _staticData
                .GetStaticData<CurrencyStaticData>()
                .GetCurrencyConfig(parameters.Type);

            for (int i = 0; i < _icons.Length; i++)
            {
                _icons[i].sprite = currencyConfig.Data.Icon;
                _icons[i].color = Color.white;
            }

            for (int i = 0; i < _rects.Length; i++)
                _rects[i].DisableElement();

            for (int i = 0; i < parameters.Count; i++)
            {
                if (i >= _rects.Length)
                    break;

                _rects[i].EnableElement();
            }

            _text.text = $"x{parameters.Count.ToString()}";
            _text.alpha = 1;
        }

        private void PlayAnimation(in CurrencyAnimationParameters parameters)
        {
            CurrencyAnimationParameters animationParameters = parameters;
            Vector3 startPosition = parameters.StartPosition; // начальная точка (в глобальных координатах)
            Vector3 endPosition = parameters.EndPosition; // конечная точка (в глобальных координатах)
            float flightDuration = _flyDuration; // длительность полета к финальной позиции
            float scatterDuration = _scatterDuration; // длительность разлета
            float textFadeDuration = 0.5f; // длительность разлета
            float spreadRange = 10f; // радиус разлета

            Sequence animationSequence = DOTween.Sequence();
            
            /*foreach (var rect in _rects)
                rect.position = startPosition;*/
            
            transform.position = startPosition;
            
            _text.alpha = 1;
            _text.DOFade(1, textFadeDuration)
                .OnComplete(() => _text.DOFade(0, textFadeDuration * 2));

            foreach (var rect in _rects)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-spreadRange, spreadRange),
                    Random.Range(-spreadRange, spreadRange),
                    0f);

                Vector3 targetPosition = startPosition + randomOffset;
                animationSequence.Join(rect.DOMove(targetPosition, scatterDuration).SetEase(Ease.OutQuad));
            }

            for (int i = 0; i < _rects.Length; i++)
            {
                RectTransform rect = _rects[i];
                Tween moveTween = rect
                        .DOMove(endPosition, flightDuration)
                        .SetEase(Ease.InOutSine)
                        .OnComplete(() => CompleteMovement(rect, animationParameters))
                    ;
                animationSequence.Insert(i * 0.1f, moveTween
                ); // Задержка между иконками
            }

            animationSequence.OnComplete(() => { Destroy(gameObject); });

            if (parameters.LinkObject != null)
                animationSequence.SetLink(parameters.LinkObject);
            animationSequence.SetLink(gameObject);
            animationSequence.Play();
        }

        private void CompleteMovement(RectTransform rect, CurrencyAnimationParameters parameters)
        {
            if (_firstObject)
            {
                parameters.StartReplenishCallback?.Invoke();
                _firstObject = false;
            }

            rect.DisableSafe();
        }
    }
}