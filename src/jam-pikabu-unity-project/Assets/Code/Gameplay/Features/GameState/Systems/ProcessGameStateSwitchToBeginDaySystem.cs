using System.Collections.Generic;
using Code.Gameplay.Features.GameState.Service;
using Code.Gameplay.Features.Loot.Factory;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Features.RoundState.Service;
using Entitas;

namespace Code.Gameplay.Features.GameState.Systems
{
    public class ProcessGameStateSwitchToBeginDaySystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly IGameStateService _gameStateService;
        private readonly IRoundStateService _roundStateService;
        private readonly IOrdersService _ordersService;
        private readonly ILootService _lootService;
        private readonly ILootFactory _lootFactory;
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _buffer = new();

        public ProcessGameStateSwitchToBeginDaySystem
        (
            GameContext context,
            IGameStateService gameStateService,
            IRoundStateService roundStateService,
            IOrdersService ordersService,
            ILootService lootService,
            ILootFactory lootFactory
        )
        {
            _gameStateService = gameStateService;
            _roundStateService = roundStateService;
            _ordersService = ordersService;
            _lootService = lootService;
            _lootFactory = lootFactory;

            _requests = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.SwitchGameStateRequest,
                    GameMatcher.BeginDay
                ));

            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.GameState
                ));
        }

        public void Execute()
        {
            foreach (var request in _requests)
            foreach (var entity in _entities.GetEntities(_buffer))
            {
                request.isDestructed = true;

                entity.ResetGameStates();
                entity.isBeginDay = true;
                entity.ReplaceGameStateTypeId(GameStateTypeId.BeginDay);

                ProcessService();
                _gameStateService.CompleteStateSwitch(GameStateTypeId.BeginDay);
                entity.isStateProcessingAvailable = true;
            }
        }

        private void ProcessService()
        {
            _roundStateService.BeginDay();
            _ordersService.InitDay(_roundStateService.CurrentDay);
            _lootFactory.CreateLootSpawner();
            _lootService.InitLootBuffer();
        }
    }
}