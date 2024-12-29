using System;
using Code.Common.Extensions;
using Code.Gameplay.Features.Currency.Config;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.StaticData;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static Code.Common.Extensions.AsyncGameplayExtensions;
using Random = UnityEngine.Random;

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
        private ISoundService _soundService;
        private bool _firstObject = true;
        private Action StartReplenishCallback;

        [Inject]
        private void Construct(IStaticDataService staticDataService, ISoundService soundService)
        {
            _soundService = soundService;
            _staticData = staticDataService;
        }

        public void Initialize(in CurrencyAnimationParameters parameters)
        {
            StartReplenishCallback = parameters.StartReplenishCallback;
            InitIcons(parameters);
            PlayAnimation(parameters);
            
            if (parameters.BeginAnimationSound is not SoundTypeId.Unknown) 
                _soundService.PlaySound(parameters.BeginAnimationSound);
        }

        private void InitIcons(in CurrencyAnimationParameters parameters)
        {
            var sprite = GetSprite(parameters);

            for (int i = 0; i < _icons.Length; i++)
            {
                _icons[i].sprite = sprite;
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

            _text.text = parameters.OverrideText 
                ? $"{parameters.TextPrefix}" 
                : $"{parameters.TextPrefix}{parameters.Count.ToString()}";

            _text.alpha = 1;
        }

        private Sprite GetSprite(in CurrencyAnimationParameters parameters)
        {
            if (parameters.Sprite != null)
                return parameters.Sprite;
            CurrencyConfig currencyConfig = _staticData
                .Get<CurrencyStaticData>()
                .GetCurrencyConfig(parameters.Type);

            Sprite sprite = currencyConfig.Data.Icon;
            return sprite;
        }

        private void PlayAnimation(in CurrencyAnimationParameters parameters)
        {
            CurrencyAnimationParameters animationParameters = parameters;
            Vector3 startPosition = parameters.StartPosition; // начальная точка (в глобальных координатах)
            Vector3 endPosition = GetEndPosition(parameters); // конечная точка (в глобальных координатах)
            float flightDuration = _flyDuration; // длительность полета к финальной позиции
            float scatterDuration = _scatterDuration; // длительность разлета
            float textFadeDuration = 0.75f; // длительность разлета
            float spreadRange = 25f; // радиус разлета
            
            Sequence animationSequence = DOTween.Sequence();

            transform.position = startPosition;
            
            TextAnimation(textFadeDuration).Forget();

            foreach (var rect in _rects)
            {
                // Случайный оффсет для разлёта
                Vector3 randomOffset = new Vector3(
                    Random.Range(-spreadRange, spreadRange),
                    Random.Range(-spreadRange, spreadRange),
                    0f);

                Vector3 targetPosition = startPosition + randomOffset;

                // Случайный угол поворота
                Vector3 randomRotation = new Vector3(0f, 0f, Random.Range(-180f, 180f));

                // Добавляем движение и поворот в последовательность
                animationSequence.Join(rect.DOMove(targetPosition, scatterDuration).SetEase(Ease.OutQuad));
                animationSequence.Join(rect.DORotate(randomRotation, scatterDuration, RotateMode.FastBeyond360).SetEase(Ease.OutQuad));
            }

            for (int i = 0; i < _rects.Length; i++)
            {
                RectTransform rect = _rects[i];
                Tween moveTween = rect
                    .DOMove(endPosition, flightDuration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() => CompleteMovement(rect, animationParameters));

                // Задержка между движениями иконок
                animationSequence.Insert(i * 0.09f, moveTween);
            }

            animationSequence.OnComplete(() => { Destroy(gameObject); });

            if (parameters.LinkObject != null)
                animationSequence.SetLink(parameters.LinkObject);

            animationSequence.SetLink(gameObject);
            animationSequence.Play();
        }

        private Vector3 GetEndPosition(in CurrencyAnimationParameters parameters)
        {
            return parameters.EndPosition;
        }

        private async UniTaskVoid TextAnimation(float textFadeDuration)
        {
            _text.alpha = 1;
            float movePosY = _text.transform.localPosition.y - 30;
            _text.transform.DOLocalMoveY(movePosY, textFadeDuration*2);
            await DelaySeconds(0.75f, _text.destroyCancellationToken);
            _text.DOFade(0, textFadeDuration);
        }

        private void CompleteMovement(RectTransform rect, CurrencyAnimationParameters parameters)
        {
            if (_firstObject)
            {
                if (parameters.StartReplenishSound is not SoundTypeId.Unknown) 
                    _soundService.PlaySound(parameters.StartReplenishSound);
             
                parameters.StartReplenishCallback?.Invoke();
                _firstObject = false;
            }

            rect.DisableSafe();
        }

        private void OnDestroy()
        {
          
        }
    }
}