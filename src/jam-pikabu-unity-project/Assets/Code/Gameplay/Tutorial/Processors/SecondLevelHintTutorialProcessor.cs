using System.Threading;
using Code.Gameplay.Tutorial.Processors.Abstract;
using Code.Gameplay.Tutorial.Window;
using Code.Gameplay.Windows;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStates;
using Code.Meta.Features.MainMenu.Windows;
using Cysharp.Threading.Tasks;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Tutorial.Processors
{
    [Injectable(typeof(ITutorialProcessor))]
    public class SecondLevelHintTutorialProcessor : BaseTutorialProcessor
    {
        public override TutorialTypeId TypeId => TutorialTypeId.SecondLevelHint;

        public override bool CanSkipTutorial()
        {
            return _daysService.GetDaysProgress().Count > 3;
        }

        public override void Finalization()
        {
        }

        protected override async UniTask ProcessInternal(CancellationToken token)
        {
            await DelaySeconds(2, token);

            if (CheckCurrentGameState<MapMenuState>() == false)
                return;
            
            await _windowService.OpenWindow<TutorialWindow>(WindowTypeId.Tutorial);

            var menu = await FindWindow<MainMenuWindow>(token);

            HideArrowTask(token, menu).Forget();

            UniTask onClickTask = menu.PlayButton.OnClickAsync(token);
            await onClickTask;

            _windowService.Close(WindowTypeId.Tutorial);
        }

        private async UniTask HideArrowTask(CancellationToken token, MainMenuWindow menu)
        {
            TutorialWindow tutorialWindow = GetCurrentWindow();

            bool arrowEnabled = false;
            while (token.IsCancellationRequested == false)
            {
                await UniTask.Yield(token);
                bool anyOtherWindowOpened = _windowService.Windows.Count > 2;
                
                if (anyOtherWindowOpened && arrowEnabled)
                {
                    tutorialWindow.HideArrow();
                    arrowEnabled = false;
                    continue;
                }

                if (anyOtherWindowOpened == false && arrowEnabled == false)
                {
                    arrowEnabled = true;
                    tutorialWindow.ShowArrow(menu.PlayButton.transform, 0, 150);
                    continue;
                }
            }
        }
    }
}