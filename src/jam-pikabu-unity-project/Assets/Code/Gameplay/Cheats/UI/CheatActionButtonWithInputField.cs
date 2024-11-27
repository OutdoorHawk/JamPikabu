using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.Cheats.UI
{
    public class CheatActionButtonWithInputField : MonoBehaviour
    {
        public TMP_InputField InputField;
        public Button Button;
        public TMP_Text ButtonText;

        public Action<string> CheatAction;

        public void Activate()
        {
            CheatAction?.Invoke(InputField.text);
        }
    }
}