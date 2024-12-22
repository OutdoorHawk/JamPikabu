using System.Linq;
using System.Threading;
using Code.Gameplay.Features.Currency.Behaviours;
using Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Tutorial.Processors.Abstract;
using Code.Gameplay.Tutorial.Window;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Factory;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStates;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.LootCollection.Service;
using Code.Meta.Features.LootCollection.ShopTab.UpgradeLoot;
using Code.Meta.Features.MainMenu.Windows;
using Code.Meta.UI.Shop.Templates;
using Code.Meta.UI.Shop.Window;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Tutorial.Processors
{
    [Injectable(typeof(ITutorialProcessor))]
    public class MetaShopBasicsTutorial : BaseTutorialProcessor
    {
        private readonly ILootCollectionService _lootCollectionService;
        private readonly IDaysService _daysService;
        private readonly IUIFactory _uiFactory;
        public override TutorialTypeId TypeId => TutorialTypeId.MetaShopBasics;

        public MetaShopBasicsTutorial(ILootCollectionService lootCollectionService, IDaysService daysService, IUIFactory uiFactory)
        {
            _daysService = daysService;
            _uiFactory = uiFactory;
            _lootCollectionService = lootCollectionService;
        }

        public override bool CanStartTutorial()
        {
            bool mapState = CheckCurrentGameState<MapMenuState>();

            if (mapState == false)
                return false;

            if (_daysService.GetDaysProgress().Count < 1)
                return false;

            return true;
        }

        public override bool CanSkipTutorial()
        {
            return _lootCollectionService.LootLevels.Values.Any(data => data.Level > 0);
        }

        public override void Finalization()
        {
            if (_windowService.TryGetWindow(out ShopWindow shopWindow))
            {
                shopWindow.BlockClosing = false;
            }
        }

        protected override async UniTask ProcessInternal(CancellationToken token)
        {
            await _windowService.OpenWindow<TutorialWindow>(WindowTypeId.Tutorial);

            var menu = await WaitForWindowToOpen<MainMenuWindow>(token);

            TutorialWindow tutorialWindow = GetCurrentWindow();
            CurrencyHolder currencyHolder = menu.GetComponentInChildren<CurrencyHolder>();
            
            await tutorialWindow
                    .HighlightObject(currencyHolder)
                    .ShowArrow(currencyHolder.PlayerCurrentGold.CurrencyIcon.transform, 0, -175, ArrowRotation.Bottom)
                    .ShowDarkBackground()
                    .ShowMessage(8, anchorType: TutorialMessageAnchorType.Top)
                    .AwaitForTapAnywhere(token, 1)
                ;

            tutorialWindow
                .ShowMessage(9, anchorType: TutorialMessageAnchorType.Top)
                .ShowDarkBackground()
                .HighlightObject(menu.ShopButton.gameObject)
                .ShowArrow(menu.ShopButton.transform, 0, 150)
                ;

            await menu.ShopButton.OnClickAsync(token);

            ResetAll();
            
            var shop = await WaitForWindowToOpen<ShopWindow>(token);
            shop.BlockClosing = true;

            await DelaySeconds(0.4f, token);

           await tutorialWindow
                .ShowMessage(10, anchorType: TutorialMessageAnchorType.Bottom)
                .AwaitForTapAnywhere(token, 1)
               ;

            LootUpgradeShopTab lootUpgradeTab = (LootUpgradeShopTab)shop.TabsContainer.TabsDictionary[ShopTabTypeId.LootUpgrade];
            LootUpgradeShopItem firstItem = lootUpgradeTab.Items.First();
            
            tutorialWindow
                .ShowMessage(11, anchorType: TutorialMessageAnchorType.Bottom)
                .ShowDarkBackground()
                .HighlightObject(firstItem.gameObject)
                .ShowArrow(firstItem.UpgradeButton.transform, 0, -150, ArrowRotation.Bottom)
                ;
            
            await firstItem.UpgradeButton.OnClickAsync(token);
            
            await UniTask.Yield(token);
            await UniTask.Yield(token);

            tutorialWindow.ClearHighlights();
            GameObject currency = _uiFactory.UIRoot.GetComponentInChildren<CurrencyAnimation>().gameObject;
            tutorialWindow.HighlightObject(currency);
            
            await tutorialWindow
                    .HideArrow()
                    .HideDarkBackground()
                    .ShowMessage(12, anchorType: TutorialMessageAnchorType.Bottom)
                    .AwaitForTapAnywhere(token, 1)
                ;
            
            shop.BlockClosing = false;
            tutorialWindow.Close();
        }

        private void ResetAll()
        {
            GetCurrentWindow()
                .ClearHighlights()
                .HideMessages()
                .HideArrow()
                .HideDarkBackground();
        }
    }
}