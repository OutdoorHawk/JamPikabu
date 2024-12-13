using System.Collections.Generic;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.Currency.Service;
using Code.Gameplay.Features.GameState.Service;
using Code.Gameplay.Features.Loot.Factory;
using Code.Gameplay.Features.Loot.Service;
using Code.Gameplay.Features.Orders.Service;
using Code.Gameplay.Features.RoundState.Service;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.GameState.Systems
{
    public class EnterBeginDayGameStateSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly IGameStateService _gameStateService;
        private readonly IRoundStateService _roundStateService;
        private readonly IOrdersService _ordersService;
        private readonly ILootService _lootService;
        private readonly ILootFactory _lootFactory;
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _buffer = new();
        private readonly IGroup<MetaEntity> _daysMeta;

        public EnterBeginDayGameStateSystem
        (
            GameContext context,
            MetaContext meta,
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
                gameState.isStateProcessingAvailable = true;
                gameState.ResetGameStates();
                
                gameState.isBeginDay = true;
                gameState.ReplaceGameStateTypeId(GameStateTypeId.BeginDay);
                
                _gameStateService.CompleteStateSwitch(GameStateTypeId.BeginDay);
            }
        }
    }
}