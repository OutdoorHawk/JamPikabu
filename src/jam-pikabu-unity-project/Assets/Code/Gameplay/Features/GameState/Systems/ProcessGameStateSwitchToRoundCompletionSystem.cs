using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.GameState.Service;
using Entitas;

namespace Code.Gameplay.Features.GameState.Systems
{
    public class ProcessGameStateSwitchToRoundCompletionSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly IGameStateService _gameStateService;
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _buffer = new();

        public ProcessGameStateSwitchToRoundCompletionSystem
        (
            GameContext context,
            IGameStateService gameStateService
        )
        {
            _gameStateService = gameStateService;

            _requests = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.SwitchGameStateRequest,
                    GameMatcher.RoundCompletion
                ));

            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.GameState,
                    GameMatcher.RoundLoop
                ));
        }

        public void Execute()
        {
            foreach (var request in _requests)
            foreach (var gameState in _entities.GetEntities(_buffer))
            {
                request.isDestructed = true;

                gameState.ResetGameStates();
                gameState.isRoundCompletion = true;
                gameState.ReplaceGameStateTypeId(GameStateTypeId.RoundCompletion);

                ProcessServices();

                _gameStateService.CompleteStateSwitch(GameStateTypeId.RoundCompletion);
                gameState.isStateProcessingAvailable = true;
            }
        }

        private void ProcessServices()
        {
            CreateGameEntity
                .Empty()
                .With(x => x.isLootEffectsApplier = true)
                .With(x => x.isAvailable = true)
                ;
        }
    }
}