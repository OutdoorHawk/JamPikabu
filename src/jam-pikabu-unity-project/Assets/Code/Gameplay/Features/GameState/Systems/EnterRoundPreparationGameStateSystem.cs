using System.Collections.Generic;
using Code.Gameplay.Features.GameState.Service;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Meta.Features.Days.Service;
using Entitas;

namespace Code.Gameplay.Features.GameState.Systems
{
    public class EnterRoundPreparationGameStateSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly IGameStateService _gameStateService;
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _buffer = new();

        public EnterRoundPreparationGameStateSystem
        (
            GameContext context,
            IGameStateService gameStateService,
            IOrdersService ordersService,
            ILootService lootService,
            IDaysService daysService
        )
        {
            _gameStateService = gameStateService;
            
            _requests = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.SwitchGameStateRequest,
                    GameMatcher.RoundPreparation
                ));

            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.GameState
                ));
        }

        public void Execute()
        {
            foreach (var request in _requests)
            foreach (var gameState in _entities.GetEntities(_buffer))
            {
                request.isDestructed = true;
                gameState.isStateProcessingAvailable = true;
                gameState.ResetGameStates();
                
                gameState.isRoundPreparation = true;
                gameState.ReplaceGameStateTypeId(GameStateTypeId.RoundPreparation);

                _gameStateService.CompleteStateSwitch(GameStateTypeId.RoundPreparation);
            }
        }
    }
}