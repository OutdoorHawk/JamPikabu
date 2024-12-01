using TMPro;
using UnityEngine;

namespace Code.Gameplay.Features.RoundState.Behaviours
{
    public class RoundStateViewBehaviour : MonoBehaviour
    {
        public TMP_Text RoundTimerText;
        
        private int _currentTime;

        public void UpdateTimer(float newTime)
        {
            if (_currentTime == (int)newTime)
                return;
            
            _currentTime = (int)newTime;
            
            RoundTimerText.text = _currentTime.ToString();
        }
    }
}