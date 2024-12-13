using System.Collections.Generic;
using Code.Gameplay.Features.GameState.Service;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Features.RoundState.Service;
using Entitas;

namespace Code.Gameplay.Features.GameState.Systems
{
    public class EnterRoundPreparationGameStateSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly IGameStateService _gameStateService;
        private readonly IOrdersService _ordersService;
        private readonly ILootService _lootService;
        private readonly IRoundStateService _roundStateService;
        
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _buffer = new();

        public EnterRoundPreparationGameStateSystem
        (
            GameContext context,
            IGameStateService gameStateService,
            IOrdersService ordersService,
            ILootService lootService,
            IRoundStateService roundStateService
        )
        {
            _gameStateService = gameStateService;
            _ordersService = ordersService;
            _lootService = lootService;
            _roundStateService = roundStateService;

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
            foreach (var entity in _entities.GetEntities(_buffer))
            {
                request.isDestructed = true;

                entity.ResetGameStates();
                entity.isRoundPreparation = true;

                entity.ReplaceGameStateTypeId(GameStateTypeId.RoundPreparation);

                ProcessServices();

                _gameStateService.CompleteStateSwitch(GameStateTypeId.RoundPreparation);
                entity.isStateProcessingAvailable = true;
            }
        }

        private void ProcessServices()
        {
            _ordersService.CreateOrder();
            _roundStateService.EnterRoundPreparation();
            _lootService.ClearCollectedLoot();
        }
    }
}