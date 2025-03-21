using System.Threading;
using Code.Gameplay.Features.GameState;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Tutorial.Processors.Abstract;
using Code.Gameplay.Tutorial.Window;
using Code.Gameplay.Windows;
using Code.Infrastructure.ABTesting;
using Code.Infrastructure.DI.Installers;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Code.Gameplay.Tutorial.Processors
{
    [Injectable(typeof(ITutorialProcessor))]
    public class TimerTutorialProcessor : BaseTutorialProcessor
    {
        private IABTestService _abTestService;
        
        private const int MESSAGE_1 = 23;
        private const int MESSAGE_REPLACE = 24;

        public override TutorialTypeId TypeId => TutorialTypeId.Timer;

        [Inject]
        private void Construct
        (
            IABTestService abTestService
        )
        {
            _abTestService = abTestService;
        }

        public override bool CanSkipTutorial()
        {
            return false;
        }

        public override void Finalization()
        {
            _inputService.PlayerInput.Enable();
        }

        protected override async UniTask ProcessInternal(CancellationToken token)
        {
            _inputService.PlayerInput.Disable();

            await WaitForGameState(GameStateTypeId.BeginDay, token);

            var tutorialWindow = await _windowService.OpenWindow<TutorialWindow>(WindowTypeId.Tutorial);
            var hud = await FindWindow<PlayerHUDWindow>(token);
            hud.TimerButton.image.raycastTarget = false;

            await tutorialWindow
                    .ShowMessage(GetMessage(), anchorType: TutorialMessageAnchorType.Top)
                    .ShowDarkBackground()
                    .HighlightObject(hud.TimerButton.gameObject)
                    .ShowArrow(hud.TimerButton.transform, 0, 150)
                    .AwaitForTapAnywhere(token, 1, 300)
                ;

            hud.TimerButton.image.raycastTarget = true;
            tutorialWindow.Close();
        }

        private int GetMessage()
        {
            if (_abTestService.GetExperimentValue(ExperimentTagTypeId.TIMER_REPLACE) is ExperimentValueTypeId.replace_timer_with_attempts)
                return MESSAGE_REPLACE;
            
            return MESSAGE_1;
        }
    }
}