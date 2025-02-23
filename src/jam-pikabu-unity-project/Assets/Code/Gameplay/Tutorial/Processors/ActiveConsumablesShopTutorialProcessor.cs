using System.Threading;
using Code.Gameplay.Tutorial.Processors.Abstract;
using Code.Gameplay.Tutorial.Window;
using Code.Gameplay.Windows;
using Code.Infrastructure.DI.Installers;
using Code.Meta.Features.Consumables.Service;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Code.Gameplay.Tutorial.Processors
{
    [Injectable(typeof(ITutorialProcessor))]
    public class ActiveConsumablesShopTutorialProcessor : BaseTutorialProcessor
    {
        private IConsumablesUIService _consumablesUIService;
        
        private const int MESSAGE_1 = 22;

        public override TutorialTypeId TypeId => TutorialTypeId.ActiveConsumablesShop;

        [Inject]
        private void Construct
        (
            IConsumablesUIService consumablesUIService
        )
        {
            _consumablesUIService = consumablesUIService;
        }

        public override bool CanStartTutorial()
        {
            return _consumablesUIService.GetActiveConsumables().Count > 0;
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
            var tutorialWindow = await _windowService.OpenWindow<TutorialWindow>(WindowTypeId.Tutorial);

            await tutorialWindow
                .ShowMessage(MESSAGE_1, anchorType: TutorialMessageAnchorType.Top)
                .ShowDarkBackground()
                .AwaitForTapAnywhere(token, 1);
            ;

            tutorialWindow.Close();
        }
    }
}