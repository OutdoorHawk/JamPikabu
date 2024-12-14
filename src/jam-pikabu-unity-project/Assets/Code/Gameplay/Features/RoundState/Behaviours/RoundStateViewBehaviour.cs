using Code.Gameplay.StaticData;
using Code.Infrastructure.View;
using Code.Meta.Features.Days.Service;
using TMPro;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.RoundState.Behaviours
{
    public class RoundStateViewBehaviour : MonoBehaviour
    {
        public TMP_Text RoundTimerText;

        private int _currentTime;
        private IStaticDataService _staticDataService;
        private IDaysService _daysService;

        [Inject]
        private void Construct(IStaticDataService staticDataService, IDaysService daysService)
        {
            _daysService = daysService;
            _staticDataService = staticDataService;
        }

        private void Awake()
        {
            _daysService.OnEnterRoundPreparation += ResetTimer;
            _daysService.OnDayBegin += ResetTimer;
        }

        private void OnDestroy()
        {
            _daysService.OnEnterRoundPreparation -= ResetTimer;
            _daysService.OnDayBegin -= ResetTimer;
        }

        private void ResetTimer()
        {
            float roundDuration = _daysService.GetDayData().RoundDuration;
            _currentTime = Mathf.RoundToInt(roundDuration);
            RoundTimerText.text = _currentTime.ToString();
        }

        public void UpdateTimer(float newTime)
        {
            if (_currentTime == (int)newTime)
                return;

            _currentTime = Mathf.RoundToInt(newTime);
            RoundTimerText.text = _currentTime.ToString();
        }
    }
}