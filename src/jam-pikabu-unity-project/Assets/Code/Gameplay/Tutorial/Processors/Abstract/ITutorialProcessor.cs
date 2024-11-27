using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.Gameplay.Tutorial.Processors.Abstract
{
    public interface ITutorialProcessor
    {
        TutorialTypeId TypeId { get; }
        bool CanStartTutorial();
        bool CanSkipTutorial();
        void Finalization();
        UniTask Process(CancellationToken token);
    }
    
}