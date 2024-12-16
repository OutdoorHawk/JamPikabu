using System.Collections.Generic;
using Code.Gameplay.Features.GameState.Service;
using Code.Gameplay.Features.Loot.Factory;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Meta.Features.Days.Service;
using Entitas;

namespace Code.Gameplay.Features.GameState.Systems
{
    public class ProcessBeginDayStateSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly IGroup<GameEntity> _completedLootSpawners;
        private readonly List<GameEntity> _buffer = new();
        private readonly IGameStateService _gameStateService;
        private readonly IDaysService _daysService;
        private readonly IOrdersService _ordersService;
        private readonly IGameplayLootService _gameplayLootService;
        private readonly ILootFactory _lootFactory;

        public ProcessBeginDayStateSystem
        (
            GameContext context,
            IGameStateService gameStateService,
            IDaysService daysService,
            IOrdersService ordersService,
            IGameplayLootService gameplayLootService,
            ILootFactory lootFactory
        )
        {
            _gameStateService = gameStateService;
            _daysService = daysService;
            _ordersService = ordersService;
            _gameplayLootService = gameplayLootService;
            _lootFactory = lootFactory;
            
            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.GameState,
                    GameMatcher.BeginDay,
                    GameMatcher.StateProcessingAvailable
                ));

            _completedLootSpawners = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.LootSpawner,
                    GameMatcher.Complete
                ));
        }

        public void Execute()
        {
            foreach (var entity in _entities.GetEntities(_buffer))
            {
                if (_completedLootSpawners.count == 0)
                    continue;
                
                entity.isStateProcessingAvailable = false;
                _gameStateService.AskToSwitchState(newState: GameStateTypeId.RoundPreparation);
            }
        }
    }
}