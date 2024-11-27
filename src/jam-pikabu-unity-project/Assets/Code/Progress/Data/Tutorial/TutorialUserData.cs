using System;
using Code.Gameplay.Tutorial;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Progress.Data.Tutorial
{
    [Serializable]
    public class TutorialUserData
    {
        [SerializeField] public int TypeInt;
        [SerializeField] public bool Completed;

        public TutorialUserData()
        {
            
        }
        
        [JsonIgnore] public TutorialTypeId Type => (TutorialTypeId)TypeInt;
    }
}