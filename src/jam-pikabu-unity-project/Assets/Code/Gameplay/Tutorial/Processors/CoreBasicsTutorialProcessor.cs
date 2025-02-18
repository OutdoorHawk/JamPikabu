using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Code.Common.Extensions;
using Code.Gameplay.Features.GameState;
using Code.Gameplay.Features.GameState.Service;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Tutorial.Components;
using Code.Gameplay.Tutorial.Processors.Abstract;
using Code.Gameplay.Tutorial.Window;
using Code.Gameplay.Windows;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStates.Game;
using Code.Meta.Features.Days.Service;
using Cysharp.Threading.Tasks;
using Zenject;
using static System.Threading.CancellationTokenSource;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Tutorial.Processors
{
    [Injectable(typeof(ITutorialProcessor))]
    public class CoreBasicsTutorialProcessor : BaseTutorialProcessor
    {
        private IGameStateService _gameStateService;

        public override TutorialTypeId TypeId => TutorialTypeId.CoreBasics;

        private const int MESSAGE_1 = 1;
        private const int MESSAGE_2 = 2;
        private const int MESSAGE_3 = 3;
        private const int MESSAGE_4 = 4;
        private const int MESSAGE_5 = 5;
        private const int MESSAGE_6 = 6;
        private const int MESSAGE_7 = 7;
        private const int MESSAGE_MOBILE_1 = 17;
        private const int MESSAGE_MOBILE_2 = 18;

        private CancellationTokenSource _destroyGameStateSource = new();

        [Inject]
        private void Construct(IGameStateService gameStateService)
        {
            _gameStateService = gameStateService;
        }

        public override bool CanStartTutorial()
        {
            bool isGameLoopState = CheckCurrentGameState<GameLoopState>();

            if (isGameLoopState == false)
                return false;

            if (_daysService.GetDayData().Id > 1)
                return false;

            return true;
        }

        public override bool CanSkipTutorial()
        {
            return _daysService.GetDaysProgress().Count > 0;
        }

        public override void Finalization()
        {
            _inputService.PlayerInput.Enable();
            StopDestroyGameState();
        }

        protected override async UniTask ProcessInternal(CancellationToken token)
        {
            _inputService.PlayerInput.Disable();

            StartDestroyGameState(token);

            await _windowService.OpenWindow<TutorialWindow>(WindowTypeId.Tutorial);
            TutorialWindow tutorialWindow = GetCurrentWindow();

            tutorialWindow
                .ShowMessage(MESSAGE_1)
                ;

            await tutorialWindow.AwaitForTapAnywhere(token, 1f);

            tutorialWindow.HideMessages();

            StopDestroyGameState();
            _gameStateService.AskToSwitchState(GameStateTypeId.BeginDay);

            await UniTask.WaitUntil(() => GetGameEntitiesGroup(GameMatcher.LootSpawner).Length != 0, cancellationToken: token);

            StartDestroyGameState(token);

            await UniTask.WaitUntil(() => GetGameEntitiesGroup(GameMatcher.LootSpawner).Length == 0, cancellationToken: token);

            var hud = await WaitForWindowToOpen<PlayerHUDWindow>(token);

            await tutorialWindow
                    .ShowMessage(MESSAGE_3)
                    .ShowArrow(hud.LootPoint, 0)
                    .AwaitForTapAnywhere(token, 1f)
                ;

            await tutorialWindow
                    .HideMessages()
                    .ShowMessage(MESSAGE_4, anchorType: TutorialMessageAnchorType.Bottom)
                    .ShowArrow(hud.HookPoint, 0, -200, ArrowRotation.Bottom)
                    .AwaitForTapAnywhere(token, 1f)
                ;

            tutorialWindow
                .HideMessages()
                .HideArrow();
            
            await PlayMobileTutorial(tutorialWindow, token);

            StopDestroyGameState();
            _gameStateService.AskToSwitchState(GameStateTypeId.RoundPreparation);

            await WaitForGameState(GameStateTypeId.RoundPreparation, token);

            await DelaySeconds(1.5f, token);

            await tutorialWindow
                    .ShowMessage(MESSAGE_5)
                    .ShowDarkBackground()
                    .HighlightObject(hud.OrderViewBehaviour.gameObject)
                    .ShowArrow(hud.OrderViewBehaviour.transform, -200, -170, ArrowRotation.Left)
                    .AwaitForTapAnywhere(token, 1f)
                ;
            
            var lootRect = hud.LootContainer.LootGrid.gameObject; //todo

            await tutorialWindow
                    .ClearHighlights()
                    .ShowMessage(MESSAGE_6, anchorType: TutorialMessageAnchorType.Bottom)
                    .ShowDarkBackground()
                    .HighlightObject(lootRect.gameObject)
                    .ShowArrow(lootRect.transform, 0, 0, ArrowRotation.Bottom)
                    .AwaitForTapAnywhere(token, 1f)
                ;

            await tutorialWindow
                    .ClearHighlights()
                    .HideDarkBackground()
                    .HideMessages()
                    .HideArrow()
                    .ShowMessage(MESSAGE_2)
                    .AwaitForTapAnywhere(token, 1f)
                ;
            
            _inputService.PlayerInput.Enable();

            tutorialWindow
                .ClearHighlights()
                .HideDarkBackground()
                .HideMessages()
                .ShowMessage(MESSAGE_7)
                ;
            
            await WaitForGameState(GameStateTypeId.RoundLoop, token);

            tutorialWindow.Close();
        }

        private async UniTask PlayMobileTutorial(TutorialWindow tutorialWindow, CancellationToken token)
        {
            if (_inputService.IsMobile() == false)
                return;

            TutorialMobileInputComponent mobileInput = tutorialWindow.ShowMobileInputTutorial();
            
            mobileInput.RightInput.EnableElement();
            mobileInput.LeftInput.EnableElement();
            mobileInput.SlideInput.DisableElement();

            await tutorialWindow
                    .ShowMessage(MESSAGE_MOBILE_1)
                    .AwaitForTapAnywhere(token, 1f)
                ;
            
            mobileInput.RightInput.DisableElement();
            mobileInput.LeftInput.DisableElement();
            mobileInput.SlideInput.EnableElement();

            await tutorialWindow
                    .ShowMessage(MESSAGE_MOBILE_2)
                    .AwaitForTapAnywhere(token, 1f)
                ;
            
            tutorialWindow.HideMobileInputTutorial();
        }

        private void StartDestroyGameState(CancellationToken token)
        {
            _destroyGameStateSource = CreateLinkedTokenSource(token);
            DestroyAllRequestsAsync().Forget();
        }

        private void StopDestroyGameState()
        {
            _destroyGameStateSource?.Cancel();
        }

        private async UniTaskVoid DestroyAllRequestsAsync()
        {
            while (_destroyGameStateSource.Token.IsCancellationRequested == false)
            {
                foreach (var gameState in GetGameEntitiesGroup(GameMatcher.AllOf(GameMatcher.SwitchGameStateRequest)))
                {
                    gameState.isSwitchGameStateRequest = false;
                    gameState.isDestructed = true;
                }

                await UniTask.Yield(_destroyGameStateSource.Token);
            }
        }

        private async UniTask WaitForGameState(GameStateTypeId state, CancellationToken token)
        {
            while (true)
            {
                GameEntity[] gameState = GetGameEntitiesGroup(GameMatcher
                    .AllOf(GameMatcher.GameState,
                        GameMatcher.GameStateTypeId));

                if (gameState.Any(x => x.GameStateTypeId == state))
                    return;

                await UniTask.Yield(token);
            }
        }
    }
}