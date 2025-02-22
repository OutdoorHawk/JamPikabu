using System.Threading;
using Code.Common.Extensions;
using Code.Gameplay.Tutorial.Processors.Abstract;
using Code.Gameplay.Tutorial.Window;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Factory;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStates;
using Code.Meta.Features.Days.Service;
using Code.Meta.Features.MainMenu.Windows;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
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

        public override bool CanSkipTutorial()
        {
            return false;
        }

        public override void Finalization()
        {
        }

        protected override async UniTask ProcessInternal(CancellationToken token)
        {
            var menu = await FindWindow<MainMenuWindow>(token, 999);

            await DelaySeconds(0.15f, token);

            var tutorial = await _windowService.OpenWindow<TutorialWindow>(WindowTypeId.Tutorial);

            Button bonusButton = menu.BonusLevelButton.Button;

            tutorial
                .ShowDarkBackground()
                .HighlightObject(bonusButton)
                .ShowArrow(bonusButton.transform, 0, 150, ArrowRotation.Top)
                .ShowMessage(13)
                ;

            await bonusButton.OnClickAsync(token);
            
            tutorial.Close();
        }
    }
}