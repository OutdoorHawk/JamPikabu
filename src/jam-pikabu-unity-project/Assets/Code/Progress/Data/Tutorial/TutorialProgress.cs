using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Progress.Data.Tutorial
{
    [Serializable]
    public class TutorialProgress
    {
        [SerializeField] public List<TutorialUserData> TutorialUserDatas = new();
    }
}