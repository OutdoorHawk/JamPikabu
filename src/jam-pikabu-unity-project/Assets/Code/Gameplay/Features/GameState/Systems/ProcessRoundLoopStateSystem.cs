using System.Collections.Generic;
using Code.Gameplay.Features.GameState.Service;
using Code.Infrastructure.States.GameStates.Game;
using Code.Infrastructure.States.StateMachine;
using Entitas;

namespace Code.Gameplay.Features.GameState.Systems
{
    public class ProcessRoundLoopStateSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _entities;
        private readonly List<GameEntity> _buffer = new();
        private readonly IGameStateService _gameStateService;
        private readonly IGroup<GameEntity> _activeRoundTimer;
        private readonly IGroup<GameEntity> _busyLoot;
        private readonly IGroup<GameEntity> _busyHook;
        private readonly IGameStateMachine _gameStateMachine;

        public ProcessRoundLoopStateSystem(GameContext context, IGameStateService gameStateService, IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
            _gameStateService = gameStateService;
            
            _entities = context.GetGroup(GameMatcher
                .AllOf(GameMatcher.GameState,
                    GameMatcher.RoundLoop,
                    GameMatcher.StateProcessingAvailable
                ));

            _activeRoundTimer = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.RoundStateController,
                    GameMatcher.RoundTimeLeft
                ));
            
            _busyLoot = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Loot,
                    GameMatcher.Busy
                ));

            _busyHook = context.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.GrapplingHook,
                    GameMatcher.Busy));
        }

        public void Execute()
        {
            foreach (var entity in _entities.GetEntities(_buffer))
            {
                if (_activeRoundTimer.count != 0)
                    continue;
                
                if (CheckHookIsStillBusy())
                    continue;

                if (CheckLootIsStillBusy())
                    continue;

                entity.isStateProcessingAvailable = false;
                _gameStateService.AskToSwitchState(newState: GameStateTypeId.RoundCompletion);
            }
        }
        
        private bool CheckHookIsStillBusy()
        {
            return _busyHook.GetEntities().Length != 0;
        }

        private bool CheckLootIsStillBusy()
        {
            return _busyLoot.GetEntities().Length != 0;
        }
    }
}