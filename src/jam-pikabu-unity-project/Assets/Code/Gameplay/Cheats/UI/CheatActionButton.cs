using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.Cheats.UI
{
    public class CheatActionButton : MonoBehaviour
    {
        public Button Button;
        public Action CheatAction;
        public TMP_Text ButtonText;
        
        public void Activate()
        {
            CheatAction?.Invoke();
        }
    }
}