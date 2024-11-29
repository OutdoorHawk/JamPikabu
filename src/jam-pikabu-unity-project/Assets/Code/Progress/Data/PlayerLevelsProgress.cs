using System;
using UnityEngine;

namespace Code.Progress.Data
{
    [Serializable]
    public class PlayerLevelsProgress
    {
        [SerializeField] public int CurrentLevelIndex;
   
        public void SetLevel(int level)
        {
            CurrentLevelIndex = level;
        }
    }
}