using System.Linq;
using System.Threading;
using Code.Gameplay.Features.GameState;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Tutorial.Processors.Abstract;
using Code.Gameplay.Tutorial.Window;
using Code.Gameplay.Windows;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStates.Game;
using Cysharp.Threading.Tasks;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Tutorial.Processors
{
    [Injectable(typeof(ITutorialProcessor))]
    public class BadIngredientsTutorialProcessor : BaseTutorialProcessor
    {
        public override TutorialTypeId TypeId => TutorialTypeId.BadIngredients;

        private const int MESSAGE_1 = 15;
        private const int MESSAGE_2 = 16;

        private CancellationTokenSource _destroyGameStateSource = new();

        public override bool CanStartTutorial()
        {
            if (_daysService.GetDayData().Id <= 1)
                return false;

            return true;
        }

        public override bool CanSkipTutorial()
        {
            return _daysService.GetDaysProgress().Count > 3;
        }

        public override void Finalization()
        {
            _inputService.PlayerInput.Enable();
            StopDestroyGameState();
        }

        protected override async UniTask ProcessInternal(CancellationToken token)
        {
            _inputService.PlayerInput.Disable();

            var tutorialWindow = await _windowService.OpenWindow<TutorialWindow>(WindowTypeId.Tutorial);

            await WaitForGameState(GameStateTypeId.RoundPreparation, token);

            StartDestroyGameState(token);

            var hud = await FindWindow<PlayerHUDWindow>(token);

            await DelaySeconds(1.5f, token);

            var lootRect = hud.LootContainer.LootGrid.gameObject;

            await tutorialWindow
                    .ClearHighlights()
                    .ShowMessage(MESSAGE_1, anchorType: TutorialMessageAnchorType.Bottom)
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

            tutorialWindow.Close();
        }

        private void StartDestroyGameState(CancellationToken token)
        {
            _destroyGameStateSource = CancellationTokenSource.CreateLinkedTokenSource(token);
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