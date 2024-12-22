using System;

namespace Code.Gameplay.Tutorial.Config
{
    [Serializable]
    public class TutorialConfig
    {
        public TutorialTypeId Type;
        public int CompletedLevelsNeedToStart;
        public int Order;
        public bool DisableTutorial;
    }
}