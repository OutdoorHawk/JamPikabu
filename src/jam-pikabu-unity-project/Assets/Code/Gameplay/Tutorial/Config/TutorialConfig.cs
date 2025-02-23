using System;
using Code.Gameplay.StaticData.Data;
using Code.Infrastructure.States.GameStates;

namespace Code.Gameplay.Tutorial.Config
{
    [Serializable]
    public class TutorialConfig
    {
        public TutorialTypeId Type;
        public GameStateTypeId GameStateType;
        public int CompletedLevelsNeedToStart;
        public int Order;
        public bool DisableTutorial;
    }
}