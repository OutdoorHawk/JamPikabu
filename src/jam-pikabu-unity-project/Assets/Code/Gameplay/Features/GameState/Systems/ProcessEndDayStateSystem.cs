using System.Collections.Generic;
using System.Threading;
using Code.Gameplay.Features.Loot.Service;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.GameStates.Game;
using Code.Infrastructure.States.StateMachine;
using Code.Meta.Features.Days.Service;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;
using static Code.Common.Extensions.AsyncGameplayExtensions;

namespace Code.Gameplay.Features.GameState.Systems
{
    public class ProcessEndDayStateSystem : IExecuteSystem, ITearDownSystem
    {
        private readonly IDaysService _roundService;
        private readonly ILootService _lootService;
        private readonly IGameStateMachine _stateMachine;

        private readonly IGroup<GameEntity> _gameState;
        private readonly List<GameEntity> _buffer = new();
        private readonly CancellationTokenSource _tearDownSource = new();

        public ProcessEndDayStateSystem
        (
            GameContext context,
            IDaysService roundService,
            ILootService lootService,
            IGameStateMachine stateMachine,
            MetaContext metaContext
        )
        {
            _roundService = roundService;
            _lootService = lootService;
            _stateMachine = stateMachine;

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

                IncreaseDayAndLoadMap(gameState).Forget();
                _lootService.ClearCollectedLoot();
            }
        }

        private async UniTaskVoid IncreaseDayAndLoadMap(GameEntity state)
        {
            await DelaySeconds(0.75f, _tearDownSource.Token);

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