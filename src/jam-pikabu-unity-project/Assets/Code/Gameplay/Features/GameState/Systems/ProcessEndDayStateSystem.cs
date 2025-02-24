using System.Collections.Generic;
using System.Threading;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Result.Service;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.GameStates.Game;
using Code.Infrastructure.States.StateMachine;
using Code.Meta.Features.Days.Service;
using Cysharp.Threading.Tasks;
using Entitas;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.GameState.Systems
{
    public class ProcessEndDayStateSystem : IExecuteSystem, ITearDownSystem
    {
        private readonly IGameplayLootService _gameplayLootService;
        private readonly IGameStateMachine _stateMachine;
        private readonly IResultWindowService _resultWindowService;

        private readonly IGroup<GameEntity> _gameState;
        private readonly List<GameEntity> _buffer = new();
        private readonly CancellationTokenSource _tearDownSource = new();

        public ProcessEndDayStateSystem
        (
            GameContext context,
            IDaysService roundService,
            IGameplayLootService gameplayLootService,
            IGameStateMachine stateMachine,
            IResultWindowService resultWindowService
        )
        {
            _gameplayLootService = gameplayLootService;
            _stateMachine = stateMachine;
            _resultWindowService = resultWindowService;

            _gameState = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.GameState,
                    GameMatcher.EndDay,
                    GameMatcher.StateProcessingAvailable
                ));
        }

        public void Execute()
        {
            foreach (var gameState in _gameState.GetEntities(_buffer))
            {
                gameState.isStateProcessingAvailable = false;

                _gameplayLootService.ClearCollectedLoot();
                IncreaseDayAndLoadMap().Forget();
            }
        }

        private async UniTaskVoid IncreaseDayAndLoadMap()
        {
            await UniTask.Yield();
            await _resultWindowService.TryShowProfitWindow();
            
            CreateGameEntity.Empty()
                .With(x => x.isSyncMetaStorageRequest = true)
                .AddGold(0);

            await DelaySeconds(0.5f, _tearDownSource.Token);
            
            _stateMachine.Enter<GameOverState>();
            await UniTask.Yield();
            _stateMachine.Enter<LoadMapMenuState>();
        }

        public void TearDown()
        {
            _tearDownSource?.Cancel();
        }
    }
}