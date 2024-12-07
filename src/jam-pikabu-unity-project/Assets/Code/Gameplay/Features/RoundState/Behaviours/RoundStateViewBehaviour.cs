using Code.Gameplay.Features.RoundState.Configs;
using Code.Gameplay.StaticData;
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

        [Inject]
        private void Construct(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }

        private void Start()
        {
            float roundDuration = _staticDataService.GetStaticData<RoundStateStaticData>().RoundDuration;
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