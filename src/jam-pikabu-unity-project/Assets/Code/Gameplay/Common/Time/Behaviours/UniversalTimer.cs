using System;
using System.Threading;
using Code.Infrastructure.Localization;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

namespace Code.Gameplay.Common.Time.Behaviours
{
    public class UniversalTimer : MonoBehaviour, ILocalizationHandler 
    {
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TimeFormat _format;

        private CancellationTokenSource _timerToken = new();
        private ILocalizationService _localizationService;

        private const string _timeFormat = "{0}{1} {2}{3}";
        private const string _timeFormatOnlySeconds = "{0}{1}";
        private const string _timeFormatManyDays = "99+{0}";

        public TMP_Text TimerText => _timerText;

        private event Action AddTimerEndCallback;

        private string _localizedDay;
        private string _localizedHour;
        private string _localizedMinutes;
        private string _localizedSeconds;

        [Inject]
        private void Construct(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        private void Start()
        {
            InitLocales();
            _localizationService.RegisterHandler(this);
        }
        
        private void OnDestroy()
        {
            _timerToken?.Cancel();
            _localizationService.UnregisterHandler(this);
        }

        public void OnLanguageChanged(Locale locale)
        {
            InitLocales();
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

        private void InitLocales()
        {
            _localizedSeconds = _localizationService["COMMON/TIMER_SECONDS"];
            _localizedMinutes = _localizationService["COMMON/TIMER_MINUTES"];
            _localizedHour = _localizationService["COMMON/TIMER_HOURS"];
            _localizedDay = _localizationService["COMMON/TIMER_DAYS"];
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
                TimeFormat.LocalizedTime => GetLocalizedTime(time),
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

        private string GetLocalizedTime(int time)
        {
            time = Mathf.Max(0, time);
            var days = time / 86400;
            time -= days * 86400;
            var hours = time / 3600;
            time -= hours * 3600;
            var min = time / 60;
            var sec = time % 60;

            if (days > 0)
            {
                return ZString.Format(_timeFormat, days, _localizedDay, hours, _localizedHour);
            }

            if (hours > 0)
            {
                if (min == 0)
                {
                    return ZString.Format(_timeFormat, hours, _localizedHour, string.Empty, string.Empty);
                }

                return ZString.Format(_timeFormat, hours, _localizedHour, min, _localizedMinutes);
            }

            if (min > 0)
            {
                return ZString.Format(_timeFormat, min, _localizedMinutes, string.Empty, string.Empty);
            }

            return ZString.Format(_timeFormatOnlySeconds, sec, _localizedSeconds);
        }
    }
}