using System;
using System.Threading;
using Code.Gameplay.Common.Time.Service;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

namespace Code.Gameplay.Common.Time.Behaviours
{
    public class UniversalTimer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TimeFormat _format;

        private CancellationTokenSource _timerToken = new();
        private ILocalizedTimeService _localizedTimeService;

        public TMP_Text TimerText => _timerText;

        private event Action AddTimerEndCallback;

        [Inject]
        private void Construct(ILocalizedTimeService localizedTimeService)
        {
            _localizedTimeService = localizedTimeService;
        }

        private void OnDestroy()
        {
            _timerToken?.Cancel();
        }

        public void OnLanguageChanged(Locale locale)
        {
        }

        public void StartTimer(Func<double> getTimeFunc, Action onTimerEnd = null)
        {
            _timerToken?.Cancel();
            _timerToken = new CancellationTokenSource();
            StartTimerTickAsync(getTimeFunc, onTimerEnd).Forget();
        }

        public void StartTimer(Func<int> getTimeFunc, Action onTimerEnd = null)
        {
            _timerToken?.Cancel();
            _timerToken = new CancellationTokenSource();
            StartTimerTickAsync(getTimeFunc, onTimerEnd).Forget();
        }

        public void RegisterAdditionalEndTimerCallback(Action onTimerEnd)
        {
            AddTimerEndCallback += onTimerEnd;
        }

        public void StopTimer()
        {
            _timerToken?.Cancel();
            AddTimerEndCallback = null;
        }

        private async UniTaskVoid StartTimerTickAsync(Func<double> getTime, Action onTimerEnd)
        {
            double time = getTime.Invoke();
            while (time > 0)
            {
                _timerText.text = FormatTime((int)time);
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _timerToken.Token);
                time = getTime.Invoke();
            }

            onTimerEnd?.Invoke();
            AddTimerEndCallback?.Invoke();
            StopTimerSelf();
        }

        private async UniTaskVoid StartTimerTickAsync(Func<int> getTime, Action onTimerEnd)
        {
            int time = getTime.Invoke();
            while (time > 0)
            {
                _timerText.text = FormatTime(time);
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _timerToken.Token);
                time = getTime.Invoke();
            }

            onTimerEnd?.Invoke();
            AddTimerEndCallback?.Invoke();
            StopTimerSelf();
        }

        private void StopTimerSelf()
        {
            StopTimer();
        }

        private string FormatTime(int time)
        {
            return _format switch
            {
                TimeFormat.LocalizedTime => _localizedTimeService.GetLocalizedTime(time),
                TimeFormat.MMSS => GetTimeInMinutesAndSeconds(time),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private string GetTimeInMinutesAndSeconds(int time)
        {
            time = Mathf.Max(0, time);
            int minutes = time / 60;
            int seconds = time % 60;

            return $"{minutes:D2}:{seconds:D2}";
        }
    }
}