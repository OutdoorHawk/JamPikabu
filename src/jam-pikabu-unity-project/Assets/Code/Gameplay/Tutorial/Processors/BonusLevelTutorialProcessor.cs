using System.Threading;
using Code.Common.Extensions;
using Code.Gameplay.Tutorial.Processors.Abstract;
using Code.Gameplay.Windows.Factory;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStates;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.MainMenu.Windows;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Tutorial.Processors
{
    [Injectable(typeof(ITutorialProcessor))]
    public class BonusLevelTutorialProcessor : BaseTutorialProcessor
    {
        private readonly IUIFactory _uiFactory;
        public override TutorialTypeId TypeId => TutorialTypeId.BonusLevel;

        public BonusLevelTutorialProcessor(IDaysService daysService, IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
        }

        public override bool CanStartTutorial()
        {
            bool mapState = CheckCurrentGameState<MapMenuState>();

            if (mapState == false)
                return false;

            return true;
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
            var menu = await WaitForWindowToOpen<MainMenuWindow>(token, 480);
            
            await DelaySeconds(2f, token);
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