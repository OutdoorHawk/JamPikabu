using System.Collections.Generic;
using Code.Gameplay.Features.GameState.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Infrastructure.States.GameStates.Game;
using Entitas;

namespace Code.Gameplay.Features.GameState.Systems
{
    public class ProcessRoundPreparationStateSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly IGroup<GameEntity> _activeRounds;
        private readonly List<GameEntity> _buffer = new();
        private readonly IGameStateService _gameStateService;
        private readonly IOrdersService _ordersService;

        public ProcessRoundPreparationStateSystem(GameContext context, 
            IGameStateService gameStateService, IOrdersService ordersService)
        {
            _gameStateService = gameStateService;
            _ordersService = ordersService;

            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.GameState,
                    GameMatcher.RoundPreparation,
                    GameMatcher.StateProcessingAvailable
                ));

            _activeRounds = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.RoundStateController,
                    GameMatcher.RoundInProcess
                ));
        }

        public void Execute()
        {
            foreach (var entity in _entities.GetEntities(_buffer))
            {
                if (_activeRounds.count == 0)
                    continue;

                ProcessServices();
                entity.isStateProcessingAvailable = false;
                _gameStateService.AskToSwitchState(newState: GameStateTypeId.RoundLoop);
            }
        }

        private void ProcessServices()
        {
         
        }
    }
}