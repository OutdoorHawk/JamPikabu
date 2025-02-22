using System.Linq;
using System.Threading;
using Code.Gameplay.Features.Consumables.Behaviours;
using Code.Gameplay.Features.GameState;
using Code.Gameplay.Features.HUD;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Tutorial.Processors.Abstract;
using Code.Gameplay.Tutorial.Window;
using Code.Gameplay.Windows;
using Code.Infrastructure.DI.Installers;
using Code.Meta.Features.Consumables;
using Code.Meta.Features.Consumables.Service;
using Code.Progress.SaveLoadService;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Tutorial.Processors
{
    [Injectable(typeof(ITutorialProcessor))]
    public class ActiveConsumablesTutorialProcessor : BaseTutorialProcessor
    {
        private IConsumablesUIService _consumablesUIService;
        private IGameplayLootService _gameplayLootService;
        private ISaveLoadService _saveLoadService;

        private const int MESSAGE_1 = 15;
        private const int MESSAGE_2 = 16;

        private bool _success = false;

        public override TutorialTypeId TypeId => TutorialTypeId.ActiveConsumables;

        [Inject]
        private void Construct
        (
            IConsumablesUIService consumablesUIService,
            IGameplayLootService gameplayLootService,
            ISaveLoadService saveLoadService
        )
        {
            _saveLoadService = saveLoadService;
            _gameplayLootService = gameplayLootService;
            _consumablesUIService = consumablesUIService;
        }

        public override bool CanSkipTutorial()
        {
            return _consumablesUIService.GetActiveConsumables().Count > 0;
        }

        public override void Finalization()
        {
            var userData = Progress.Tutorial.TutorialUserDatas.Find(config => config.Type == TypeId);

            if (_success == false)
            {
                userData.Completed = false;
                _saveLoadService.SaveProgress();
            }
        }

        protected override async UniTask ProcessInternal(CancellationToken token)
        {
            await WaitForGameState(GameStateTypeId.BeginDay, token);

            await DelaySeconds(1, token);

            _gameplayLootService.SpawnLoot(LootTypeId.Spoon);
            _gameplayLootService.SpawnLoot(LootTypeId.Spoon);

            await WaitForCollectConsumable(token);

            var tutorialWindow = await _windowService.OpenWindow<TutorialWindow>(WindowTypeId.Tutorial);
            var hud = await FindWindow<PlayerHUDWindow>(token);

            if (hud.ConsumablesHolder.ButtonsDict.TryGetValue(ConsumableTypeId.Spoon, out ConsumableBoosterButton button) == false)
                return;

            Transform buttonTransform = button.transform;
            Button consumableButton = button.Button;

            tutorialWindow
                .ShowMessage(MESSAGE_1, anchorType: TutorialMessageAnchorType.Bottom)
                .ShowDarkBackground()
                .HighlightObject(buttonTransform.gameObject)
                .ShowArrow(buttonTransform.transform, -175, 0, ArrowRotation.Right)
                ;

            await consumableButton.OnClickAsync(token);

            await tutorialWindow
                    .ClearHighlights()
                    .HideDarkBackground()
                    .HideMessages()
                    .HideArrow()
                    .ShowMessage(MESSAGE_2)
                    .AwaitForTapAnywhere(token, 1f)
                ;

            tutorialWindow.Close();
            _success = true;
        }

        private async UniTask WaitForCollectConsumable(CancellationToken token)
        {
            await UniTask.WaitUntil(() => _consumablesUIService.GetActiveConsumables().Count > 0, cancellationToken: token);
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