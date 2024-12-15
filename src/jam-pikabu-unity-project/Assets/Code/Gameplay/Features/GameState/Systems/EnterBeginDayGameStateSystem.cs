using System.Collections.Generic;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Features.GameState.Service;
using Code.Gameplay.Features.Loot.Factory;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Meta.Features.Days.Service;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.GameState.Systems
{
    public class EnterBeginDayGameStateSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly IGameStateService _gameStateService;
        private readonly IDaysService _daysService;
        private readonly IOrdersService _ordersService;
        private readonly ILootService _lootService;
        private readonly ILootFactory _lootFactory;
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _buffer = new();

        public EnterBeginDayGameStateSystem
        (
            GameContext context,
            MetaContext meta,
            IGameStateService gameStateService,
            IDaysService daysService,
            IOrdersService ordersService,
            ILootService lootService,
            ILootFactory lootFactory
        )
        {
            _gameStateService = gameStateService;
            _daysService = daysService;
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
            foreach (var gameState in _entities.GetEntities(_buffer))
            {
                request.isDestructed = true;
                gameState.isStateProcessingAvailable = true;
                gameState.ResetGameStates();
                
                gameState.isBeginDay = true;
                gameState.ReplaceGameStateTypeId(GameStateTypeId.BeginDay);
                
                _gameStateService.CompleteStateSwitch(GameStateTypeId.BeginDay);
            }
        }
    }
}