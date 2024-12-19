using System.Threading;
using Code.Gameplay.Tutorial.Processors.Abstract;
using Code.Infrastructure.DI.Installers;
using Cysharp.Threading.Tasks;

namespace Code.Gameplay.Tutorial.Processors
{
    [Injectable(typeof(ITutorialProcessor))]
    public class CoreBasicsTutorialProcessor : BaseTutorialProcessor
    {
        public override TutorialTypeId TypeId => TutorialTypeId.CoreBasics;
        
        public override bool CanStartTutorial()
        {
            return false;
        }

        public override bool CanSkipTutorial()
        {
            return false;
        }

        public override void Finalization()
        {
            
        }

        protected override async UniTask ProcessInternal(CancellationToken token)
        {
         
        }
    }
}