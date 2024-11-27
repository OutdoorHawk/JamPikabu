using System;
using UnityEngine;

namespace Code.Progress.Data
{
    [Serializable]
    public class PlayerLevelsProgress
    {
        [SerializeField] public int CurrentLevelIndex;
        [SerializeField] public int MaxWavesInEndlessMode;

        public void SetLevel(int level)
        {
            CurrentLevelIndex = level;
        }
    }
}