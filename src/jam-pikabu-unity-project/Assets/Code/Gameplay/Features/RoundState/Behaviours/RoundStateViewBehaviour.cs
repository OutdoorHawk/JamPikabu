using Code.Gameplay.Features.RoundState.Configs;
using Code.Gameplay.Features.RoundState.Service;
using Code.Gameplay.StaticData;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.RoundState.Behaviours
{
    public class RoundStateViewBehaviour : MonoBehaviour
    {
        public TMP_Text RoundTimerText;

        private int _currentTime;
        private IStaticDataService _staticDataService;
        private IRoundStateService _roundStateService;

        [Inject]
        private void Construct(IStaticDataService staticDataService, IRoundStateService roundStateService)
        {
            _roundStateService = roundStateService;
            _staticDataService = staticDataService;
        }

        private void Awake()
        {
            _roundStateService.OnEnterRoundPreparation += ResetTimer;
            _roundStateService.OnDayBegin += ResetTimer;
        }

        private void OnDestroy()
        {
            _roundStateService.OnEnterRoundPreparation -= ResetTimer;
            _roundStateService.OnDayBegin -= ResetTimer;
        }
        
        private void ResetTimer()
        {
            float roundDuration = _roundStateService.GetDayData().RoundDuration;
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