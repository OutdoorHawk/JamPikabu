using System.Threading;
using Code.Gameplay.Tutorial.Processors.Abstract;
using Code.Gameplay.Tutorial.Window;
using Code.Gameplay.Windows;
using Code.Infrastructure.DI.Installers;
using Code.Meta.Features.Consumables.Service;
using Code.Meta.Features.MainMenu.Windows;
using Code.Meta.UI.Shop.Window;
using Code.Meta.UI.Shop.WindowService;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Code.Gameplay.Tutorial.Processors
{
    [Injectable(typeof(ITutorialProcessor))]
    public class ActiveConsumablesShopTutorialProcessor : BaseTutorialProcessor
    {
        private IConsumablesUIService _consumablesUIService;
        private IShopWindowService _shopWindowService;

        private const int MESSAGE_1 = 22;

        public override TutorialTypeId TypeId => TutorialTypeId.ActiveConsumablesShop;

        [Inject]
        private void Construct
        (
            IConsumablesUIService consumablesUIService,
            IShopWindowService shopWindowService
        )
        {
            _shopWindowService = shopWindowService;
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
            var menu = await FindWindow<MainMenuWindow>(token);
            var tutorialWindow = await _windowService.OpenWindow<TutorialWindow>(WindowTypeId.Tutorial);

            tutorialWindow
                .ShowMessage(MESSAGE_1, anchorType: TutorialMessageAnchorType.Top)
                .ShowDarkBackground()
                .HighlightObject(menu.ShopButton.gameObject)
                .ShowArrow(menu.ShopButton.transform, 0, 150)
                ;

            await menu.ShopButton.OnClickAsync(token);

            await FindWindow<ShopWindow>(token);
            await UniTask.Yield(token);

            _shopWindowService.SetTabSelected(ShopTabTypeId.Consumables);

            tutorialWindow.Close();
        }
    }
}