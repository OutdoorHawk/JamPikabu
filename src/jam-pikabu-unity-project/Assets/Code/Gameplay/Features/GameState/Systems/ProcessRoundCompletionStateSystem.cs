using System.Collections.Generic;
using Code.Gameplay.Features.GameOver.Service;
using Code.Gameplay.Features.GameState.Service;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Service;
using Entitas;

namespace Code.Gameplay.Features.GameState.Systems
{
    public class ProcessRoundCompletionStateSystem : IExecuteSystem
    {
        private readonly IGameStateService _gameStateService;
        private readonly IOrdersService _ordersService;
        private readonly IGameOverService _gameOverService;
        private readonly ILootService _lootService;
        private readonly IGroup<GameEntity> _entities;
        private readonly IGroup<GameEntity> _activeOrders;
        private readonly List<GameEntity> _buffer = new();

        public ProcessRoundCompletionStateSystem
        (
            GameContext context,
            IGameStateService gameStateService,
            IOrdersService ordersService,
            IGameOverService gameOverService,
            ILootService lootService
        )
        {
            _gameStateService = gameStateService;
            _ordersService = ordersService;
            _gameOverService = gameOverService;
            _lootService = lootService;

            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.GameState,
                    GameMatcher.RoundCompletion,
                    GameMatcher.StateProcessingAvailable
                ));

            _activeOrders = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.Order,
                    GameMatcher.ResultProcessed
                ));
        }

        public void Execute()
        {
            foreach (var gameState in _entities.GetEntities(_buffer))
            foreach (var order in _activeOrders)
            {
                gameState.isStateProcessingAvailable = false;
                CheckDayEndConditions(order);
            }
        }

        private void CheckDayEndConditions(GameEntity order)
        {
            order.isDestructed = true;
            
            if (_ordersService.CheckAllOrdersCompleted() == false)
            {
                GoToNextOrder();
                return;
            }

            _gameStateService.AskToSwitchState(GameStateTypeId.EndDay);
        }

        private void GoToNextOrder()
        {
            _ordersService.GoToNextOrder();
            _gameStateService.AskToSwitchState(newState: GameStateTypeId.RoundPreparation);
        }
    }
}