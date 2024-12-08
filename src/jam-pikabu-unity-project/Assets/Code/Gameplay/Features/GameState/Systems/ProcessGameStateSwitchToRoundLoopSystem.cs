using System.Collections.Generic;
using Code.Gameplay.Features.GameState.Service;
using Entitas;

namespace Code.Gameplay.Features.GameState.Systems
{
    public class ProcessGameStateSwitchToRoundLoopSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly IGameStateService _gameStateService;
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _buffer = new();

        public ProcessGameStateSwitchToRoundLoopSystem(GameContext context, IGameStateService gameStateService)
        {
            _gameStateService = gameStateService;

            _requests = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.SwitchGameStateRequest,
                    GameMatcher.RoundLoop
                ));

            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.GameState,
                    GameMatcher.RoundPreparation
                ));
        }

        public void Execute()
        {
            foreach (var request in _requests)
            foreach (var entity in _entities.GetEntities(_buffer))
            {
                request.isDestructed = true;

                entity.ResetGameStates();
                entity.isRoundLoop = true;
                entity.ReplaceGameStateTypeId(GameStateTypeId.RoundLoop);

                _gameStateService.CompleteStateSwitch(GameStateTypeId.RoundLoop);
                entity.isStateProcessingAvailable = true;
            }
        }
    }
}