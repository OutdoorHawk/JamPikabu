using TMPro;
using UnityEngine;

namespace Code.Gameplay.Features.RoundState.Behaviours
{
    public class RoundStateViewBehaviour : MonoBehaviour
    {
        public TMP_Text RoundTimerText;

        private int _currentValue = -1;

        public void UpdateRoundStateCounter(float newValue)
        {
            if (_currentValue == (int)newValue)
                return;

            _currentValue = Mathf.RoundToInt(newValue);
            RoundTimerText.text = _currentValue.ToString();
        }
    }
}