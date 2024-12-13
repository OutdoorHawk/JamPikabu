using System.Collections.Generic;
using Code.Gameplay.Features.GameState.Service;
using Entitas;

namespace Code.Gameplay.Features.GameState.Systems
{
    public class EnterEndDayGameStateSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly IGameStateService _gameStateService;
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _buffer = new();
        private readonly IGroup<MetaEntity> _daysMeta;

        public EnterEndDayGameStateSystem
        (
            GameContext context,
            IGameStateService gameStateService,
            MetaContext meta
        )
        {
            _gameStateService = gameStateService;

            _requests = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.SwitchGameStateRequest,
                    GameMatcher.EndDay
                ));

            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.GameState,
                    GameMatcher.RoundCompletion
                ));

            _daysMeta = meta.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.Day
                ));
        }

        public void Execute()
        {
            foreach (var request in _requests)
            foreach (var gameState in _entities.GetEntities(_buffer))
            foreach (var day in _daysMeta)
            {
                request.isDestructed = true;

                gameState.ResetGameStates();
                gameState.isEndDay = true;
                gameState.ReplaceGameStateTypeId(GameStateTypeId.EndDay);

                _gameStateService.CompleteStateSwitch(GameStateTypeId.EndDay);
                gameState.isStateProcessingAvailable = true;
            }
        }
    }
}