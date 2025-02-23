using Code.Common.Extensions;
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
        private IDaysService _daysService;

        [Inject]
        private void Construct(IDaysService daysService)
        {
            _daysService = daysService;
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
            if (_daysService.CanShowTimer() == false)
            {
                RoundTimerText.gameObject.DisableElement();
                return;
            }

            float roundDuration = _daysService.GetRoundDuration();
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