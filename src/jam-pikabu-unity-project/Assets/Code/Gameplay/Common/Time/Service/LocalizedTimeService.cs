using Code.Infrastructure.Localization;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Code.Gameplay.Common.Time.Service
{
    public class LocalizedTimeService : ILocalizedTimeService, ILocalizationHandler
    {
        private readonly ILocalizationService _localizationService;

        private const string _timeFormat = "{0}{1} {2}{3}";
        private const string _timeFormatOnlySeconds = "{0}{1}";
        private const string _timeFormatManyDays = "99+{0}";

        private string _localizedDay;
        private string _localizedHour;
        private string _localizedMinutes;
        private string _localizedSeconds;

        public LocalizedTimeService(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public void OnLanguageChanged(Locale locale)
        {
            InitLocalesAsync().Forget();
        }

        public string GetLocalizedTime(int time)
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

        private async UniTask InitLocalesAsync()
        {
            await LocalizationSettings.InitializationOperation.Task;
            await UniTask.Yield();
            _localizedSeconds = _localizationService["COMMON/TIMER_SECONDS"];
            _localizedMinutes = _localizationService["COMMON/TIMER_MINUTES"];
            _localizedHour = _localizationService["COMMON/TIMER_HOURS"];
            _localizedDay = _localizationService["COMMON/TIMER_DAYS"];
        }
    }
}