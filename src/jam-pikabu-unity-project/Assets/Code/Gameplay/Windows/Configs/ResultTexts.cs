using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Gameplay.Windows.Configs
{
    [Serializable]
    public class ResultTexts
    {
        [SerializeField] private List<string> _newRecordWinTexts;
        [SerializeField] private List<string> _winNoRecordTexts;
        [SerializeField] private List<string> _loseTexts;

        public string NewRecordWinText => _newRecordWinTexts[Random.Range(0, _newRecordWinTexts.Count)];
        public string WinNoRecordText => _winNoRecordTexts[Random.Range(0, _winNoRecordTexts.Count)];
        public string LoseText => _loseTexts[Random.Range(0, _loseTexts.Count)];
    }
}