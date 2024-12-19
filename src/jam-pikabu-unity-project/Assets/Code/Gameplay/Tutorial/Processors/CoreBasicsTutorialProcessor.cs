using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Code.Gameplay.Features.GameState;
using Code.Gameplay.Features.GameState.Service;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Tutorial.Processors.Abstract;
using Code.Gameplay.Tutorial.Window;
using Code.Gameplay.Windows;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStates.Game;
using Cysharp.Threading.Tasks;
using Zenject;

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

        [Inject]
        private void Construct(IGameStateService gameStateService)
        {
            _gameStateService = gameStateService;
        }

        public override bool CanStartTutorial()
        {
            bool isGameLoopState = CheckCurrentGameState<GameLoopState>();

            if (isGameLoopState)
                return true;

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
            foreach (var gameState in GetGameEntitiesGroup(GameMatcher.AllOf(GameMatcher.SwitchGameStateRequest)))
            {
                gameState.isSwitchGameStateRequest = false;
                gameState.isDestructed = true;
            }

            await _windowService.OpenWindow<TutorialWindow>(WindowTypeId.Tutorial);
            TutorialWindow tutorialWindow = GetCurrentWindow();

            tutorialWindow
                .ShowMessage(MESSAGE_1)
                ;

            await tutorialWindow.AwaitForTapAnywhere(token);

            tutorialWindow.ShowMessage(MESSAGE_2);

            await tutorialWindow.AwaitForTapAnywhere(token);
            
            _gameStateService.AskToSwitchState(GameStateTypeId.RoundPreparation);

            await WaitForGameState(GameStateTypeId.RoundPreparation, token);

            var hud = await WaitForWindowToOpen<PlayerHUDWindow>(token);

            var boxRect = hud.gameObject; //todo

            await tutorialWindow
                    .HighlightObject(boxRect)
                    .ShowMessage(MESSAGE_3)
                    .AwaitForTapAnywhere(token)
                ;
            
            
            
            var lootRect = hud.LootContainer.LootGrid.gameObject; //todo
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