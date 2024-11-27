using System;

namespace Code.Gameplay.Tutorial.Service
{
    public interface ITutorialService
    {
        event Action OnTutorialUpdate;
        void Initialize();
        void SkipCurrentTutorial();
        bool IsTutorialStartedOrCompleted(TutorialTypeId type);
    }
}